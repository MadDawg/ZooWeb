using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;

namespace ZooWeb.Pages.Animals
{
    [Authorize(Roles = "admin, zookeeper")]
    public class IndexModel : PageModel
    {
        public List<AnimalInfo> listAnimals = new List<AnimalInfo>();
        public void OnGet()
        {
            //try
            //{
            string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "SELECT * FROM animal";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AnimalInfo info = new AnimalInfo();
                            info.Animal_Id = reader.GetInt32(0).ToString();
                            info.Name = reader.GetString(1);
                            info.Scientific_name = reader.GetString(2);

                            //if (reader.IsDBNull(3)) { info.Super_Eid = "NULL"; } else { info.Super_Eid = reader.GetInt32(3).ToString(); }

                            info.Common_name = reader.GetString(3);
                            if (reader.GetBoolean(4)) { info.Sex = "male"; } else { info.Sex = "female"; }
                            info.Birth_date = reader.GetDateTime(5).Date;
                            info.Status = reader.GetString(6);
                            info.Location_Id = reader.GetInt64(7).ToString();

                            listAnimals.Add(info);
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

    public class AnimalInfo
    {
        public string Animal_Id;
        public string Name;
        public string Scientific_name;
        public string Common_name;
        public string Sex;
        public DateTime Birth_date;
        public string Status;
        public string Location_Id;
    }
}
//hello