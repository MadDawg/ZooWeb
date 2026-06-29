using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using Microsoft.Data.SqlClient;
using System.Numerics;

namespace ZooWeb.Pages.ZooUsers
{
    public class IndexModel : PageModel
    {
        public List<ZooUserInfo> listZooUsers = new List<ZooUserInfo>();
        public void OnGet()
        {
            string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "SELECT UserID, Username, IsActive, CreationDate " 
                    + "FROM zoo_user " +
                    "WHERE UserRole <> 'system'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ZooUserInfo info = new ZooUserInfo();
                            info.UserId = reader.GetInt32(0).ToString();
                            info.Username = reader.GetString(1);
                            if (reader.GetBoolean(2)) { info.IsActive = "enabled"; } else { info.IsActive = "disabled"; }
							info.CreationDate = reader.GetDateTime(3).ToString();
                            
							listZooUsers.Add(info);
                        }
                    }
                }
            }
        }
    }

    public class ZooUserInfo
    {
        public string UserId;
        public string Username;
        public string PasswordHash;
        public string IsActive;
        public string CreationDate;
        public string UserRole;
    }
}