using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class ZooUserRepository{

    private readonly IDbConnectionFactory _factory;

    public ZooUserRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public async Task<ZooUserInfo?> GetByIdAsync(int id){
      using var connection = _factory.CreateConnection();

      await connection.OpenAsync();

      await using var command = connection.CreateCommand();
      command.CommandText = """
        SELECT *
        FROM zoo_user
        WHERE UserID=@id AND UserRole <> 'system'
        """;

      command.Parameters.AddWithValue("@id", id);

      await using var reader = await command.ExecuteReaderAsync();

      if(!await reader.ReadAsync()){ return null; }

      return new ZooUserInfo
      {
        UserId = reader.GetInt32(reader.GetOrdinal("UserID")).ToString(),
        Username = reader.GetString(reader.GetOrdinal("Username")),
        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")) ? "enabled": "disabled",
        CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")).ToString(),
        UserRole = reader.GetString(reader.GetOrdinal("UserRole"))
      };
    }

    public async Task<int?> GetIdByUsernameAsync(string username){
      using var connection = _factory.CreateConnection();

      await connection.OpenAsync();

      await using var command = connection.CreateCommand();
      command.CommandText = """
        SELECT UserId
        FROM zoo_user
        WHERE Username=@username AND UserRole <> 'system'
        """;

      command.Parameters.AddWithValue("@username", username);

      await using var reader = await command.ExecuteReaderAsync();

      if(!await reader.ReadAsync()){ return null; }

      return reader.GetInt32(reader.GetOrdinal("UserID"));
    }

    public async Task<ZooUserValidatorInfo?> GetByUsernameForValidationAsync(string username){
      using var connection = _factory.CreateConnection();

      await connection.OpenAsync();

      await using var command = connection.CreateCommand();
      command.CommandText = """
        SELECT PasswordHash, UserRole 
        FROM zoo_user
        WHERE Username=@username AND UserRole <> 'system' AND IsActive = 1
        """;

      command.Parameters.AddWithValue("@username", username);

      await using var reader = await command.ExecuteReaderAsync();

      if(!await reader.ReadAsync()){ return null; }

      return new ZooUserValidatorInfo{
        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
        UserRole = reader.GetString(reader.GetOrdinal("UserRole"))
      };
    }

    public async Task<List<ZooUserInfo>> GetAllAsync(){
      List<ZooUserInfo> ZooUsers = new List<ZooUserInfo>();

      using var connection = _factory.CreateConnection();

      await connection.OpenAsync();

      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT UserID, Username, IsActive, CreationDate, UserRole
        FROM zoo_user
        WHERE UserRole <> 'system'
        """;

      await using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        ZooUserInfo entity = new ZooUserInfo();
        entity.UserId = reader.GetInt32(reader.GetOrdinal("UserID")).ToString();
        entity.Username = reader.GetString(reader.GetOrdinal("Username"));
        // entity.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
        entity.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")) ? "enabled": "disabled";
        entity.CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")).ToString();
        entity.UserRole = reader.GetString(reader.GetOrdinal("UserRole"));

        ZooUsers.Add(entity);
      }
      return ZooUsers;
    }

    public async Task AddAsync(ZooUserInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();

      await using var command = connection.CreateCommand();

      command.CommandText = """
        INSERT INTO zoo_user (Username, PasswordHash, IsActive, UserRole)
        VALUES (@Username, @Password, @Status, @UserRole)
        """;

      command.Parameters.AddWithValue("@Username", entity.Username);
      command.Parameters.AddWithValue("@Password", entity.PasswordHash);
      command.Parameters.AddWithValue("@Status", true); // defaults to false on the database side
      command.Parameters.AddWithValue("@UserRole", entity.UserRole);

      await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(ZooUserInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();

      await using var command = connection.CreateCommand();
      command.CommandText = """
        UPDATE zoo_user
        SET Username=@Username, IsActive=@Status
        """;

      if (!String.IsNullOrEmpty(entity.PasswordHash)) {
        command.CommandText +=  ", PasswordHash=@Password ";
      }
      if (!String.IsNullOrEmpty(entity.UserRole))
      {
        command.CommandText += ", UserRole=@UserRole ";
      }
      command.CommandText += "WHERE UserId=@UserId";

      command.Parameters.AddWithValue("@UserId", entity.UserId);
      command.Parameters.AddWithValue("@Username",entity.Username);

      if (!String.IsNullOrEmpty(entity.PasswordHash))
      {
        command.Parameters.AddWithValue("@Password", entity.PasswordHash);
      }
      if (!String.IsNullOrEmpty(entity.UserRole))
      {
        command.Parameters.AddWithValue("@UserRole", entity.UserRole);
      }
      command.Parameters.AddWithValue("@Status", (entity.IsActive == "enabled"));

      await command.ExecuteNonQueryAsync();
    }

    public async Task ToggleAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();

      await using var command = connection.CreateCommand();

      command.CommandText = """
        UPDATE zoo_user
        SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END
        WHERE UserId = @id;
        """;

      command.Parameters.AddWithValue("@id", id);
      
      await command.ExecuteNonQueryAsync();
    }
  }
}

