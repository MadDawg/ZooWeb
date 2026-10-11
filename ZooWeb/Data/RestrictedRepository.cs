
using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class RestrictedRepository{

    private readonly IDbConnectionFactory _factory;

    public RestrictedRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }
    
    public async Task<RestrictedInfo?> GetByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        SELECT *
        FROM restricted
        WHERE Location_ID = @id;
        """;
      
      command.Parameters.AddWithValue("@id", id);

      await using var reader = await command.ExecuteReaderAsync();
      if (!await reader.ReadAsync()){ return null; }

      return new RestrictedInfo{
        LocationId = reader.GetInt64(reader.GetOrdinal("Location_ID")).ToString(),
        CloseDate = reader.GetDateTime(reader.GetOrdinal("Close_date")),
        ReopenDate = reader.GetDateTime(reader.GetOrdinal("Reopen_date"))
      };
    }

    public async Task<List<RestrictedInfo>> GetAllAsync(){
      List<RestrictedInfo> restricted_enclosures = new List<RestrictedInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      command.CommandText = """
        SELECT * 
        FROM restricted
        """;
      
      await using var reader = await command.ExecuteReaderAsync();
  
      while (await reader.ReadAsync()){
        RestrictedInfo entity = new RestrictedInfo{
          LocationId = reader.GetInt64(reader.GetOrdinal("Location_ID")).ToString(),
          CloseDate = reader.GetDateTime(reader.GetOrdinal("Close_date")),
          ReopenDate = reader.GetDateTime(reader.GetOrdinal("Reopen_date"))
        };
        restricted_enclosures.Add(entity);
      }
      return restricted_enclosures;
    }
    
    public async Task AddAsync(RestrictedInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        INSERT INTO restricted (Location_ID, Close_date, Reopen_date)
        VALUES (@LocationId, @CloseDate, @ReopenDate)
        """;

      command.Parameters.AddWithValue("@LocationId", entity.LocationId);
      command.Parameters.AddWithValue("@CloseDate", entity.CloseDate);
      command.Parameters.AddWithValue("@ReopenDate", entity.ReopenDate);

      await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(RestrictedInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        UPDATE restricted
        SET Close_date=@CloseDate, Reopen_date=@ReopenDate
        WHERE Location_ID=@id
        """;

      command.Parameters.AddWithValue("@id", entity.LocationId);
      command.Parameters.AddWithValue("@CloseDate", entity.CloseDate);
      command.Parameters.AddWithValue("@ReopenDate", entity.ReopenDate);
      
      await command.ExecuteNonQueryAsync();
    }
    
    public async Task DeleteByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        DELETE restricted WHERE Location_ID = @id;
        """;

      command.Parameters.AddWithValue("@id", id);
      
      await command.ExecuteNonQueryAsync();
    }
  }
}
