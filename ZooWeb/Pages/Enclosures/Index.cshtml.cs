using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;

namespace ZooWeb.Pages.Enclosures
{
    [Authorize(Roles = "admin, zookeeper")]
    public class IndexModel : PageModel
    {
        public List<EnclosureInfo> listEnclosures = new List<EnclosureInfo>();
        public void OnGet()
        {
            string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "SELECT * FROM enclosure";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EnclosureInfo info = new EnclosureInfo();
                            info.LocationID = reader.GetInt64(0).ToString();
                            info.Type = reader.GetString(1);
                            info.Capacity = reader.GetInt32(2).ToString();
                            info.Occupant_Num = reader.GetInt32(3).ToString();

                            listEnclosures.Add(info);
                        }
                    }
                }
            }
        }
    }

    public class EnclosureInfo
    {
        public string LocationID;
        public string Type;
        public string Capacity;
        public string Occupant_Num;
    }
}