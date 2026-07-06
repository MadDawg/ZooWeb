using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;


namespace ZooWeb.Pages.Enclosures
{
  [Authorize(Roles = "admin, zookeeper")]
  public class IndexModel : PageModel
  {
    public List<EnclosureInfo> listEnclosures = new List<EnclosureInfo>();

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
    public IActionResult OnPostDelete(int id){

      using var connection = _factory.CreateConnection();
      connection.Open();

      const string sql = "DELETE restricted WHERE LocationID=@id";

      using var command = new SqlCommand(sql, connection);
      command.Parameters.AddWithValue("@id", id);
      command.ExecuteNonQuery();

      return RedirectToPage();
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
