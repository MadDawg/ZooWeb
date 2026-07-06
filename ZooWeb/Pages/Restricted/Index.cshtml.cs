using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;


namespace ZooWeb.Pages.Restricted
{
  [Authorize(Roles = "admin, zookeeper")]
  public class IndexModel : PageModel
  {
    public List<RestrictedInfo> Restrictions = new List<RestrictedInfo>();

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

    public IActionResult OnPostDelete(int id){
      
      using var connection = _factory.CreateConnection();
      connection.Open();
			
      const string sql = "DELETE restricted WHERE Location_ID=@id";

      using var command = new SqlCommand(sql, connection);
      command.Parameters.AddWithValue("@id", id);
      command.ExecuteNonQuery();

      return RedirectToPage();
    }
  }

  public class RestrictedInfo
  {
    public string Location_ID;
    public DateTime Close_date;
    public DateTime Reopen_date;
  }
}  
