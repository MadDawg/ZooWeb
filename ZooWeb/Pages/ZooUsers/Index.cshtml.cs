using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;


namespace ZooWeb.Pages.ZooUsers
{
  public class IndexModel : PageModel
  {
    public List<ZooUserInfo> listZooUsers = new List<ZooUserInfo>();

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
    public IActionResult OnPostToggleStatus(int id){
      using var connection = _factory.CreateConnection();
      connection.Open();

      const string sql = """
        UPDATE zoo_user
        SET IsValid = CASE WHEN IsValid = 1 THEN 0 ELSE 1 END
        WHERE UserId = @id;
      """;

      using var command = new SqlCommand(sql, connection);
      command.Parameters.AddWithValue("@id", id);
      command.ExecuteNonQuery();

      return RedirectToPage();
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
