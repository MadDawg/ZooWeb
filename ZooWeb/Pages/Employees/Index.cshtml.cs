using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using System.Numerics;

namespace ZooWeb.Pages.Employees
{
    [Authorize(Policy = "admin")]
    public class IndexModel : PageModel
    {
        public List<EmployeeInfo> listEmployees = new List<EmployeeInfo>();
        public void OnGet()
        {

            //try
            //
                string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

                using (SqlConnection connection = new SqlConnection(connectionString)) 
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
            //}
            //catch(Exception ex)
            //{
               // Console.WriteLine("Exception: " + ex.ToString());
            //}
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
