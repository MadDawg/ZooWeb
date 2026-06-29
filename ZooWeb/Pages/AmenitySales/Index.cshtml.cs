using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;


namespace ZooWeb.Pages.AmenitySales
{
	[Authorize(Policy = "admin")]
	public class IndexModel : PageModel
    {
        public List<AmenitytSalesInfo> ListAmentySales = new List<AmenitytSalesInfo>();

        public void OnGet()
        {

            string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "SELECT * " +
                    "FROM amenitySales";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AmenitytSalesInfo info = new AmenitytSalesInfo();
                            info.EID = reader.GetInt32(0).ToString();
                            info.LocationID = reader.GetInt32(1).ToString();
                            info.SaleType = reader.GetString(2);
                            info.SaleDate = reader.GetDateTime(3).ToString("yyyy-MM-dd");
							info.SaleTotal = reader.GetSqlMoney(4).ToString();
                            info.SaleId = reader.GetInt64(5).ToString();

                            ListAmentySales.Add(info);
                        }
                    }
                }
            }
        }
    }
        
}
public class AmenitytSalesInfo
{
    public string EID;
    public string LocationID;
    public string SaleType;
    public string SaleDate;
    public string SaleTotal;
    public string SaleId;
}