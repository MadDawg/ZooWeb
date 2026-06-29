using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ZooWeb.Pages.Employees
{
    public class EditModel : PageModel
    {
        public EmployeeInfo info = new EmployeeInfo();
        public List<ListTable> employeeList = new List<ListTable>();
        public List<ListTable> departmentList = new List<ListTable>();
        public string errorMsg = "";
        public string successMsg = "";
        public void OnGet()
        {
            string id = Request.Query["id"]; //this is the primary key

            try
            {
                string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM employee WHERE EmployeeId=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                info.EmployeeId = reader.GetInt32(0).ToString();
                                info.Phone_num = reader.GetString(1);
                                info.Dno = reader.GetInt16(2).ToString();

                                if (reader.IsDBNull(3)) { info.Super_Eid = "NULL"; } else { info.Super_Eid = reader.GetInt32(3).ToString(); }

                                info.Email = reader.GetString(4);
                                info.Fname = reader.GetString(5);
                                info.Lname = reader.GetString(6);
                                info.Salary = reader.GetInt32(7).ToString();
                            }
                        }
                    }

                    sql = "SELECT EmployeeId, FName, Lname "
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
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
        }

        public void OnPost() 
        {
            info.EmployeeId = Request.Form["Eid"];
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

            foreach (FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(info);
                string[] excludedFields = { "Super_Eid" };
                if (!excludedFields.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
                {
                    errorMsg = "All fields are required";
                    return;
                }
            }

			try
            {
                string connectionString = "Server=tcp:zoowebdbserver.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE employee " +
                        "SET Phone_num=@Phone_num, Dno=@Dno, Super_Eid=@Super_Eid, Email=@Email, Fname=@Fname, Lname=@Lname, Salary=@Salary " +
                        "WHERE EmployeeId=@EmployeeId"; //primary key
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeId", info.EmployeeId);
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
            successMsg = "Employee Edited Successfully";

            Response.Redirect("/Employees/Index");
        }    
    }
}
