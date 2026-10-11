using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class EnclosureRepository{

    private readonly IDbConnectionFactory _factory;

    public EnclosureRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }
    
    public async Task<EnclosureInfo?> GetByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM enclosure
        WHERE LocationID = @id
        """;

      command.Parameters.AddWithValue("@id", id);
      
      await using var reader = await command.ExecuteReaderAsync();
      if (!await reader.ReadAsync()){ return null; }

      return new EnclosureInfo{
        LocationId = reader.GetInt64(reader.GetOrdinal("LocationID")).ToString(),
        Type = reader.GetString(reader.GetOrdinal("Type")),
        Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")).ToString(),
        OccupantNumber = reader.GetInt32(reader.GetOrdinal("OccupantNum")).ToString()
      };
    }

    public async Task<List<EnclosureInfo>> GetAllAsync(){
      List<EnclosureInfo> enclosures = new List<EnclosureInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        SELECT *
        FROM enclosure
        """;
      
      await using var reader = await command.ExecuteReaderAsync();
      
      while (await reader.ReadAsync()){
        EnclosureInfo entity = new EnclosureInfo{
          LocationId = reader.GetInt64(reader.GetOrdinal("LocationID")).ToString(),
          Type = reader.GetString(reader.GetOrdinal("Type")),
          Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")).ToString(),
          OccupantNumber = reader.GetInt32(reader.GetOrdinal("OccupantNum")).ToString()
        };
        enclosures.Add(entity);
      }
      return enclosures;
    }
    
    public async Task<List<ListTable>> GetAllMinimalAsync(){
      List<ListTable> enclosures = new List<ListTable>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT LocationId, Type
        FROM enclosure
        """;
      
      await using var reader = await command.ExecuteReaderAsync();
      
      while (await reader.ReadAsync()){
        ListTable entity = new ListTable{
          Key = reader.GetInt32(reader.GetOrdinal("LocationId")).ToString(),
          Display = reader.GetString(reader.GetOrdinal("Type"))
        };
        enclosures.Add(entity); 
      }
      return enclosures;
    }
    
    public async Task AddAsync(EnclosureInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        INSERT INTO  enclosure (Type, Capacity, OccupantNum)
        VALUES (@Type, @Capacity, @OccupantNumber)
        """;

      command.Parameters.AddWithValue("@Type", entity.Type);
      command.Parameters.AddWithValue("@Capacity", entity.Capacity);
      command.Parameters.AddWithValue("@OccupantNumber", entity.OccupantNumber);

      await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(EnclosureInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        UPDATE enclosure
        SET Type=@Type, Capacity=@Capacity, OccupantNum=@OccupantNumber
        WHERE LocationID = @id
        """;

      command.Parameters.AddWithValue("@id", entity.LocationId);
      command.Parameters.AddWithValue("@Type", entity.Type);
      command.Parameters.AddWithValue("@Capacity", entity.Capacity);
      command.Parameters.AddWithValue("@OccupantNumber", entity.OccupantNumber);
      
      await command.ExecuteNonQueryAsync();
    }
    
    public async Task DeleteByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        DELETE enclosure WHERE LocationID = @id
        """;

      await using var reader = await command.ExecuteReaderAsync();
      command.Parameters.AddWithValue("@id", id);

      await command.ExecuteNonQueryAsync();
    }
  }
}
