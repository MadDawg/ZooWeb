using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using System.Diagnostics;
using ZooWeb.Data;


namespace ZooWeb.Pages.FeedingPatterns
{
  [Authorize(Roles = "admin, zookeeper")]
  public class IndexModel : PageModel
  {
    public List<FeedingPatternInfo> listFeedingPatterns = new List<FeedingPatternInfo>();

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
        String sql = "SELECT * FROM feeding_pattern";
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
          using (SqlDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              FeedingPatternInfo info = new FeedingPatternInfo();
              info.Animal_ID = reader.GetInt32(0).ToString();
              info.Meal = reader.GetString(1);
              info.Portion = reader.GetDecimal(2).ToString();
              info.Schedule_days = reader.GetString(3).Split(',').ToList();
              info.Schedule_time = reader.GetTimeSpan(4);

              listFeedingPatterns.Add(info);
            }
          }
        }
      }
    }
    
    public IActionResult OnPostDelete(int id){

      using var connection = _factory.CreateConnection();
      connection.Open();

      const string sql = "DELETE restricted WHERE Animal_ID=@id";

      using var command = new SqlCommand(sql, connection);
      command.Parameters.AddWithValue("@id", id);
      command.ExecuteNonQuery();

      return RedirectToPage();
    }
  }

  public class FeedingPatternInfo
  {
    public string Animal_ID;
    public string Meal;
    public string Portion;
    public List<string> Schedule_days;
    public TimeSpan Schedule_time;
  }
}
