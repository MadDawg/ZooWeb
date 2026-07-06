using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;


namespace ZooWeb.Pages.Employees
{
  [Authorize(Policy = "admin")]
  public class IndexModel : PageModel
  {
    public List<EmployeeInfo> listEmployees = new List<EmployeeInfo>();

    private readonly IDbConnectionFactory _factory;

    public IndexModel(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public void OnGet()
    {

      using (SqlConnection connection = _factory.CreateConnection())
      { 
        connection.Open();
        String sql = "SELECT * FROM employee";
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
          using (SqlDataReader reader = command.ExecuteReader())
          {
            while (reader.Read()) 
            { 
              EmployeeInfo info = new EmployeeInfo();
              info.EmployeeId = reader.GetInt32(0).ToString();
              info.Phone_num = reader.GetString(1);
              info.Dno = reader.GetInt16(2).ToString();

              if (reader.IsDBNull(3)){info.Super_Eid = "NULL";} else {info.Super_Eid = reader.GetInt32(3).ToString();}

              info.Email = reader.GetString(4);
              info.Fname = reader.GetString(5);
              info.Lname = reader.GetString(6);
              info.Salary = reader.GetInt32(7).ToString();

              listEmployees.Add(info);
            }
          }
        }
      }
    }
    public IActionResult OnPostToggleStatus(int id){
      using var connection = _factory.CreateConnection();
      connection.Open();

      const string sql = """
        UPDATE employee
        SET IsEmployed = CASE WHEN IsEmployed = 1 THEN 0 ELSE 1 END
        WHERE UserId = @id;
      """;

      using var command = new SqlCommand(sql, connection);
      command.Parameters.AddWithValue("@id", id);
      command.ExecuteNonQuery();

      return RedirectToPage();
    }
  }

  public class EmployeeInfo
  {
    public string EmployeeId;
    public string Phone_num;
    public string Dno;
    public string? Super_Eid;
    public string Email;
    public string Fname;
    public string Lname;
    public string Salary;
  }
}  
