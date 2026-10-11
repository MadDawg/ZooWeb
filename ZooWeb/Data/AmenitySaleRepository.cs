
using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class AmenitySaleRepository{

    private readonly IDbConnectionFactory _factory;

    public AmenitySaleRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }
    
    public async Task<AmenitySaleInfo?> GetByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM amenitySales
        WHERE SaleId = @id
        """;
      
      command.Parameters.AddWithValue("@id", id);
      
      await using var reader = await command.ExecuteReaderAsync();
      if (!await reader.ReadAsync()){ return null; }

      return new AmenitySaleInfo{
        EmployeeId = reader.GetInt32(reader.GetOrdinal("Eid")).ToString(),
        LocationId = reader.GetInt32(reader.GetOrdinal("LocationID")).ToString(),
        SaleType = reader.GetString(reader.GetOrdinal("SaleType")),
        SaleDate = reader.GetDateTime(reader.GetOrdinal("SaleDate")).ToString("yyyy-MM-dd"),
        SaleTotal = reader.GetSqlMoney(reader.GetOrdinal("SaleTotal")).ToString(),
        SaleId = reader.GetInt64(reader.GetOrdinal("SaleId")).ToString(),
        IsValid = reader.GetBoolean(reader.GetOrdinal("IsValid")) ? "valid sale" : "invalid sale"
      };
    }

    public async Task<List<AmenitySaleInfo>> GetAllAsync(){
      List<AmenitySaleInfo> amenitySales = new List<AmenitySaleInfo>();
      
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        SELECT *
        FROM amenitySales
        """;
      
      await using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        AmenitySaleInfo entity = new AmenitySaleInfo{
          EmployeeId = reader.GetInt32(reader.GetOrdinal("Eid")).ToString(),
          LocationId = reader.GetInt32(reader.GetOrdinal("LocationID")).ToString(),
          SaleType = reader.GetString(reader.GetOrdinal("SaleType")),
          SaleDate = reader.GetDateTime(reader.GetOrdinal("SaleDate")).ToString("yyyy-MM-dd"),
          SaleTotal = reader.GetSqlMoney(reader.GetOrdinal("SaleTotal")).ToString(),
          SaleId = reader.GetInt64(reader.GetOrdinal("SaleId")).ToString(),
          IsValid = reader.GetBoolean(reader.GetOrdinal("IsValid")) ? "valid sale" : "invalid sale"
        };
        amenitySales.Add(entity);
      }
      return amenitySales;
    }
    
    public async Task AddAsync(AmenitySaleInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        INSERT INTO amenitySales (Eid, LocationID, SaleType, SaleTotal)
        VALUES (@EID, @LocationID, @SaleType, @SaleTotal)
        """;

      command.Parameters.AddWithValue("@EID", entity.EmployeeId);
      command.Parameters.AddWithValue("@LocationID", entity.LocationId);
      command.Parameters.AddWithValue("@SaleType", entity.SaleType);
      command.Parameters.AddWithValue("@SaleTotal", entity.SaleTotal);

      await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(AmenitySaleInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        UPDATE amenitySales
        SET Eid=@EID, SaleType=@SaleType, LocationID=@LocationID
        WHERE SaleId=@id
        """;

      command.Parameters.AddWithValue("@EID", entity.EmployeeId);
      command.Parameters.AddWithValue("@LocationID", entity.LocationId);
      command.Parameters.AddWithValue("@SaleType", entity.SaleType);
      command.Parameters.AddWithValue("@SaleId", entity.SaleId);
      
      await command.ExecuteNonQueryAsync();
    }
    
    public async Task ToggleAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        UPDATE amenitySales
        SET IsValid = CASE WHEN IsValid = 1 THEN 0 ELSE 1 END
        WHERE SaleId = @id;
        """;

      command.Parameters.AddWithValue("@id", id);
      
      await command.ExecuteNonQueryAsync();
    }
  }
}
