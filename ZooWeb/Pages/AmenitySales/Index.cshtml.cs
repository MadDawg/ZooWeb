using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;


namespace ZooWeb.Pages.AmenitySales
{
  [Authorize(Policy = "admin")]
  public class IndexModel : PageModel
  {
    public List<AmenitytSalesInfo> ListAmentySales = new List<AmenitytSalesInfo>();

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
              if (reader.GetBoolean(4)) { info.IsValid = "valid sale"; } else { info.IsValid = "invalid sale"; }

              ListAmentySales.Add(info);
            }
          }
        }
      }
    }
    public IActionResult OnPostToggleStatus(int id){
      using var connection = _factory.CreateConnection();
      connection.Open();

      const string sql = """
        UPDATE amenitySales
        SET IsValid = CASE WHEN IsValid = 1 THEN 0 ELSE 1 END
        WHERE SaleId = @id;
      """;

      using var command = new SqlCommand(sql, connection);
      command.Parameters.AddWithValue("@id", id);
      command.ExecuteNonQuery();

      return RedirectToPage();
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
  public string IsValid;
}
