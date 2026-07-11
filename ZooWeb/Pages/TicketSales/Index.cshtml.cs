using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ZooWeb.Data;


namespace ZooWeb.Pages.TicketSales
{
  [Authorize(Policy = "admin")]
  public class IndexModel : PageModel
  {
    public List<TicketSaleInfo> ListTicketSales = new List<TicketSaleInfo>();

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
        String sql = "SELECT * FROM ticket_sales";
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
          using (SqlDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              TicketSaleInfo info = new TicketSaleInfo();
              info.TicketID = reader.GetInt32(0).ToString();
              info.PassType = reader.GetString(1);
              info.EmployeeID = reader.GetInt32(2).ToString();
              info.VisitorPn = reader.GetInt64(3).ToString();
              info.SaleTotal = reader.GetSqlMoney(5).ToString();
              info.SaleDate = reader.GetDateTime(4).ToString();
              if (reader.GetBoolean(6)) { info.IsValid = "valid"; } else { info.IsValid = "invalid"; }

              ListTicketSales.Add(info);
            }
          }
        }
      }
    }

    public IActionResult OnPostToggleStatus(int id){
      using var connection = _factory.CreateConnection();
      connection.Open();

      const string sql = """
        UPDATE ticket_sales
        SET IsValid = CASE WHEN IsValid = 1 THEN 0 ELSE 1 END
        WHERE Ticket_Id = @id;
      """;

      using var command = new SqlCommand(sql, connection);
      command.Parameters.AddWithValue("@id", id);
      command.ExecuteNonQuery();


      return RedirectToPage();
    }
  }

  public class TicketSaleInfo
  {
    public string TicketID;
    public string PassType;
    public string EmployeeID;
    public string VisitorPn;
    public string SaleTotal;
    public string SaleDate;
    public string IsValid;
  }
}
