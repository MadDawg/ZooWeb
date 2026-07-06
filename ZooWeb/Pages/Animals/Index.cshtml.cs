using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;


namespace ZooWeb.Pages.Animals
{
  [Authorize(Roles = "admin, zookeeper")]
  public class IndexModel : PageModel
  {
    public List<AnimalInfo> listAnimals = new List<AnimalInfo>();

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
              info.Common_name = reader.GetString(3);
              if (reader.GetBoolean(4)) { info.Sex = "male"; } else { info.Sex = "female"; }
              info.Birth_date = reader.GetDateTime(5).Date;
              info.Status = reader.GetString(6);
              info.Location_Id = reader.GetInt64(7).ToString();
              if (reader.GetBoolean(4)) { info.IsDeleted = "not deleted"; } else { info.Sex = "deleted"; }

              listAnimals.Add(info);
            }
          }
        }
      }
    }
    public IActionResult OnPostToggleStatus(int id){
      using var connection = _factory.CreateConnection();
      connection.Open();

      const string sql = """
        UPDATE animal
        SET IsDeleted = CASE WHEN IsDeleted = 1 THEN 0 ELSE 1 END
        WHERE Animal_ID = @id;
      """;

      using var command = new SqlCommand(sql, connection);
      command.Parameters.AddWithValue("@id", id);
      command.ExecuteNonQuery();

      return RedirectToPage();
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
    public string IsDeleted;
  }
}
//hello
