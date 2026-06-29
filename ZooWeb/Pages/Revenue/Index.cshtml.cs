using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using System.Numerics;

namespace ZooWeb.Pages.Revenue
{
    [Authorize(Policy = "admin")]
    public class IndexModel : PageModel
    {
        public List<revenueInfo> listRevenue = new List<revenueInfo>();
        public void OnGet()
        {

            //try
            //
                string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

                using (SqlConnection connection = new SqlConnection(connectionString)) 
                { 
                    connection.Open();
                    String sql = "SELECT * FROM Revenue";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) 
                            { 
                                revenueInfo info = new revenueInfo();
                                info.Total = reader.GetDecimal(0).ToString();
                                info.ReceiptSource = reader.GetString(1);
							    info.ReceiptNum = reader.GetString(2);
                                info.RevenueDate = reader.GetDateTime(3).ToString();

                                listRevenue.Add(info);
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

    public class revenueInfo
    {
        public string Total;
        public string ReceiptSource;
        public string ReceiptNum;
        public string RevenueDate;
        public string Eid;
	}
}