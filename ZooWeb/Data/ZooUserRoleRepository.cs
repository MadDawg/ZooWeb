using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data
{
  public class ZooUserRoleRepository{
    private readonly IDbConnectionFactory _factory;

    public ZooUserRoleRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public async Task<string?> GetByIdAsync(int id){ 
      using var connection = _factory.CreateConnection();

      await connection.OpenAsync();

      await using var command = connection.CreateCommand();
      command.CommandText = """
        SELECT RoleName
        FROM zoo_user_role
        WHERE RoleId = @id AND RoleName <> 'system'
        """;

      command.Parameters.AddWithValue("@id", id);

      await using var reader = await command.ExecuteReaderAsync();

      if (!await reader.ReadAsync()){ return null; }

      return reader.GetString(reader.GetOrdinal("RoleName"));
    }

    public async Task<string?> GetByNameAsync(string name){
      using var connection = _factory.CreateConnection();

      await connection.OpenAsync();

      await using var command = connection.CreateCommand();
      command.CommandText = """
        SELECT RoleName
        FROM zoo_user_role
        WHERE RoleName = @name AND RoleName <> 'system'
        """;

      command.Parameters.AddWithValue("@name", name);

      await using var reader = await command.ExecuteReaderAsync();

      if (!await reader.ReadAsync()){ return null; }

      return reader.GetString(reader.GetOrdinal("RoleName"));
    }

    public async Task<List<ListTable>> GetAllAsync(){
      List<ListTable> userRoles = new List<ListTable>();
      using var connection = _factory.CreateConnection();

      await connection.OpenAsync();

      await using var command = connection.CreateCommand();
      command.CommandText = """
        SELECT RoleName
        FROM zoo_user_role
        WHERE RoleName <> 'system'
        """;


      await using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        var roleName = reader.GetString(reader.GetOrdinal("RoleName"));
        userRoles.Add(new ListTable{ Key = roleName, Display = roleName });
      }

      return userRoles;
    }

    public async Task AddAsync(string name){
      using var connection = _factory.CreateConnection();

      await connection.OpenAsync();

      await using var command = connection.CreateCommand();
      command.CommandText = """
        INSERT INTO zoo_user_role (RoleName) VALUES (@name)
        """;

      await using var reader = await command.ExecuteReaderAsync();
      command.Parameters.AddWithValue("@name", name);

      await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteByIdAsync(int id){
      using var connection = _factory.CreateConnection();

      await connection.OpenAsync();

      await using var command = connection.CreateCommand();
      command.CommandText = """
        DELETE zoo_user_role WHERE RoleId = @id
        """;

      await using var reader = await command.ExecuteReaderAsync();
      command.Parameters.AddWithValue("@id", id);

      await command.ExecuteNonQueryAsync();

    }

    public async Task DeleteByNameAsync(string name){
      using var connection = _factory.CreateConnection();

      await connection.OpenAsync();

      await using var command = connection.CreateCommand();
      command.CommandText = """
        DELETE zoo_user_role WHERE RoleName = @name
        """;

      await using var reader = await command.ExecuteReaderAsync();
      command.Parameters.AddWithValue("@name", name);

      await command.ExecuteNonQueryAsync();
    }
  }
}
