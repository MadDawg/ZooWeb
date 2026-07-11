using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using ZooWeb.Pages.Employees;
using ZooWeb.Data;


namespace ZooWeb.Pages.Home
{
  public class PrivacyModel : PageModel
  {
    private readonly ILogger<PrivacyModel> _logger;
    private readonly IDbConnectionFactory _factory;

    public PrivacyModel(ILogger<PrivacyModel> logger, IDbConnectionFactory factory)
    {
      _logger = logger;
      _factory = factory;
    }

    public List<notification> notifications = new List<notification>();
    public int count = 0;
    public void OnGet()
    {
      string username = User?.FindFirstValue(ClaimTypes.Name);
      int userid = 0;

      if (!string.IsNullOrEmpty(username))
      {
        using (SqlConnection connection = _factory.CreateConnection())
        {
          connection.Open();
          string sql = "SELECT UserId FROM zoo_user WHERE Username=@username";
          using (SqlCommand command = new SqlCommand(sql, connection))
          {
            command.Parameters.AddWithValue("@username", username);

            using (SqlDataReader reader = command.ExecuteReader())
            {
              while (reader.Read())
              {
                userid = reader.GetInt32(0);
              }
            }
          }
        }

        using (SqlConnection connection = _factory.CreateConnection())
        {
          connection.Open();
          string sql = "SELECT MessageId, Title, Message, Timestamp FROM notification WHERE Recipient=@userid";
          using (SqlCommand command = new SqlCommand(sql, connection))
          {
            command.Parameters.AddWithValue("@userid", userid);

            using (SqlDataReader reader = command.ExecuteReader())
            {
              while (reader.Read())
              {
                notification info = new notification();
                info.messageId = reader.GetInt32(0);
                info.title = reader.GetString(1);
                info.message = reader.GetString(2);
                info.timestamp = reader.GetDateTime(3);
                count++;

                notifications.Add(info);
              }
            }
          }
        }
      }
    }
    public IActionResult OnPostDelete(int id){

      using var connection = _factory.CreateConnection();
      connection.Open();

      const string sql = "DELETE restricted WHERE MessageId=@id";

      using var command = new SqlCommand(sql, connection);
      command.Parameters.AddWithValue("@id", id);
      command.ExecuteNonQuery();

      return RedirectToPage();
    }
  }

  public class notification
  {
    public int messageId { get; set; }
    public string title { get; set; }
    public string message { get; set; }
    public DateTime timestamp { get; set; }

  }
}
