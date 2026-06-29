using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;

namespace ZooWeb.Pages.Restricted
{
    [Authorize(Roles = "admin, zookeeper")]
    public class IndexModel : PageModel
    {
        public List<RestrictedInfo> Restrictions = new List<RestrictedInfo>();
        public void OnGet()
        {
            //try
            //{
                string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

                using (SqlConnection connection = new SqlConnection(connectionString)) 
                { 
                    connection.Open();
                    String sql = "SELECT * FROM restricted";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) 
                            { 
                                RestrictedInfo info = new RestrictedInfo();
                                info.Location_ID = reader.GetInt64(0).ToString();
                                info.Close_date = reader.GetDateTime(1);
                                info.Reopen_date = reader.GetDateTime(2);

                            Restrictions.Add(info);
                            }
                        }
                    }
                }
        }
    }

    public class RestrictedInfo
    {
        public string Location_ID;
        public DateTime Close_date;
        public DateTime Reopen_date;
    }
}  
//hello
