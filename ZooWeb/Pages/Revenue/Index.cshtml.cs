using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;


namespace ZooWeb.Pages.Revenue
{
    [Authorize(Policy = "admin")]
    public class IndexModel : PageModel
    {
        public List<revenueInfo> listRevenue = new List<revenueInfo>();
        
        private readonly IDbConnectionFactory _factory;

        public IndexModel(IDbConnectionFactory factory)
        {
          _factory = factory;
        }

        public void OnGet()
        {

            //try
            //
                using (SqlConnection connection = _factory.CreateConnection())
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
