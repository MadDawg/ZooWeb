using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class NotificationRepository{
    private readonly IDbConnectionFactory _factory;

    public NotificationRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public async Task<List<NotificationInfo>> GetByUserIdAsync(int id){
      var notifications = new List<NotificationInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT MessageId, Title, Message, Timestamp FROM notification WHERE Recipient=@userid
        """;

      command.Parameters.AddWithValue("@userid", id);

      await using var reader = await command.ExecuteReaderAsync();

      while(await reader.ReadAsync()){
        var entity = new NotificationInfo{
          messageId = reader.GetInt32(reader.GetOrdinal("MessageId")),
          title = reader.GetString(reader.GetOrdinal("Title")),
          message = reader.GetString(reader.GetOrdinal("Message")),
          timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
        };
        notifications.Add(entity);
      }
      return notifications;
    }

    public async Task DeleteByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        DELETE notification WHERE MessageId = @id;
      """;

      command.Parameters.AddWithValue("@id", id);

      await command.ExecuteNonQueryAsync();
    }
  }
}
