using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using ZooWeb.Data;


namespace ZooWeb.Pages.Employees
{
  public class CreateModel : PageModel
  {
    public EmployeeInfo info = new EmployeeInfo();
    public List<ListTable> employeeList = new List<ListTable>();
    public List<ListTable> departmentList = new List<ListTable>();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly IDbConnectionFactory _factory;

    public CreateModel(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public void OnGet()
    {
      using (SqlConnection connection = _factory.CreateConnection())
      {
        connection.Open();
        String sql = "SELECT EmployeeId, FName, Lname "
          + "FROM employee";
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
          using (SqlDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              employeeList.Add(new ListTable
                  {
                  Key = reader.GetInt32(0).ToString(),
                  Display = reader.GetString(2) + ", " + reader.GetString(1)
                  });
            }
          }
        }

        sql = "SELECT * "
          + "FROM department";
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
          using (SqlDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              departmentList.Add(new ListTable
                  {
                  Key = reader.GetInt16(0).ToString(),
                  Display = reader.GetString(1)
                  });
            }
          }
        }
      }
    }

    public void OnPost() 
    {
      //must add check for null later

      //info.EmployeeId = "0";
      info.Phone_num = Request.Form["Phone_num"];

      Regex rx = new Regex(@"^\d{10}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

      if (!rx.IsMatch(info.Phone_num))
      {
        errorMsg = "Phone number must be all numbers with no spaces";
        return;
      }

      info.Dno = Request.Form["Dno"];
      info.Super_Eid = Request.Form["Super_Eid"];
      info.Email = Request.Form["Email"];
      info.Fname = Request.Form["Fname"];
      info.Lname = Request.Form["Lname"];
      info.Salary = Request.Form["Salary"];

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
      string[] excludedFields = { "EmployeeId", "Super_Eid", "IsEmployed" };
      foreach (FieldInfo field in fields)
      {
        object fieldValue = field.GetValue(info);
        if (!excludedFields.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
        {
          errorMsg = "Missing required field: " + field.Name;
          return;
        }
      }

      try
      {
        using (SqlConnection connection = _factory.CreateConnection())
        {
          connection.Open();
          String sql = "SELECT TOP 1 * FROM employee ORDER BY EmployeeId DESC;";
          using (SqlCommand command = new SqlCommand(sql, connection))
          {
            using (SqlDataReader reader = command.ExecuteReader())
            {
              while (reader.Read())
              {
                info.EmployeeId = (reader.GetInt32(0) + 1).ToString();
              }
            }
          }
        }

        using (SqlConnection connection = _factory.CreateConnection())
        {
          connection.Open();
          string sql = "INSERT INTO employee (Phone_num, Dno, Super_Eid, Email, Fname, Lname, Salary)" +
            "VALUES (@Phone_num, @Dno, @Super_Eid, @Email, @Fname, @Lname, @Salary)";

          using (SqlCommand command = new SqlCommand(sql, connection))
          {
            command.Parameters.AddWithValue("@Phone_num", info.Phone_num);
            command.Parameters.AddWithValue("@Dno", info.Dno);
            command.Parameters.AddWithValue("@Super_Eid", info.Super_Eid);
            command.Parameters.AddWithValue("@Email", info.Email);
            command.Parameters.AddWithValue("@Fname", info.Fname);
            command.Parameters.AddWithValue("@Lname", info.Lname);
            command.Parameters.AddWithValue("@Salary", info.Salary);

            command.ExecuteNonQuery();
          }
        }
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        return;
      }

      foreach (FieldInfo field in fields)
      {
        field.SetValue(info, "");
      }
      successMsg = "New Employee Added";

      Response.Redirect("/Employees/Index");
    }
  }
  public class ListTable
  {
    public string Key { get; set; }
    public string Display { get; set; }
  }
}
