using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class AnimalRepository{

    private readonly IDbConnectionFactory _factory;

    public AnimalRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public async Task<AnimalInfo?> GetByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      command.CommandText = """
        SELECT *
        FROM animal
        WHERE Animal_ID=@id
        """;

      command.Parameters.AddWithValue("@id", id);

      await using var reader = await command.ExecuteReaderAsync();
      if (!await reader.ReadAsync()){ return null; }

      return new AnimalInfo
      {
        AnimalId = reader.GetInt32(reader.GetOrdinal("Animal_ID")).ToString(),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        ScientificName = reader.GetString(reader.GetOrdinal("Scientific_name")),
        CommonName = reader.GetString(reader.GetOrdinal("Common_name")),
        Sex = reader.GetBoolean(reader.GetOrdinal("Sex")) ? "male": "female",
        BirthDate = reader.GetDateTime(reader.GetOrdinal("Birth_date")),
        Status = reader.GetString(reader.GetOrdinal("Status")),
        LocationId = reader.GetInt64(reader.GetOrdinal("Location_ID")).ToString(),
        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")) ? "true": "false"
      };
    }

    public async Task<List<AnimalInfo>> GetAllAsync(){
      List<AnimalInfo> animals = new List<AnimalInfo>();

      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM animal
        """;

      await using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        AnimalInfo entity = new AnimalInfo{
          AnimalId = reader.GetInt32(reader.GetOrdinal("Animal_ID")).ToString(),
          Name = reader.GetString(reader.GetOrdinal("Name")),
          ScientificName = reader.GetString(reader.GetOrdinal("Scientific_name")),
          CommonName = reader.GetString(reader.GetOrdinal("Common_name")),
          Sex = reader.GetBoolean(reader.GetOrdinal("Sex")) ? "male": "female",
          BirthDate = reader.GetDateTime(reader.GetOrdinal("Birth_date")),
          Status = reader.GetString(reader.GetOrdinal("Status")),
          LocationId = reader.GetInt64(reader.GetOrdinal("Location_ID")).ToString(),
          IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")) ? "true": "false"
        };
        animals.Add(entity);
      }
      return animals;
    }

    public async Task<List<ListTable>> GetAllMinimalAsync(){
      List<ListTable> animals = new List<ListTable>();

      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT Animal_ID, Name, Common_name
        FROM animal
        """;

      await using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        ListTable entity = new ListTable{
          Key = reader.GetInt32(reader.GetOrdinal("Animal_ID")).ToString(),
          Display = $"""{reader.GetString(reader.GetOrdinal("Name"))} ({reader.GetString(reader.GetOrdinal("Common_name"))})"""
        };
        animals.Add(entity);
      }
      return animals;
    }


    public async Task AddAsync(AnimalInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        INSERT INTO animal (Name, Scientific_name, Common_name, Sex, Birth_date, Status, Location_ID)
        VALUES (@Name, @ScientificName, @CommonName, @Sex, @BirthDate, @Status, @LocationId)
        """;

      command.Parameters.AddWithValue("@Name", entity.Name);
      command.Parameters.AddWithValue("@ScientificName", entity.ScientificName);
      command.Parameters.AddWithValue("@CommonName", entity.CommonName);
      command.Parameters.AddWithValue("@Sex", (entity.Sex == "male"));
      command.Parameters.AddWithValue("@BirthDate", entity.BirthDate);
      command.Parameters.AddWithValue("@Status", entity.Status);
      command.Parameters.AddWithValue("@LocationId", long.Parse(entity.LocationId));

      await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(AnimalInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        UPDATE animal
        SET Name = @Name, Scientific_name = @ScientificName, Common_name = @CommonName, Sex = @Sex, Birth_date = @BirthDate, Status = @Status, Location_ID = @LocationId
        WHERE Animal_ID = @id
        """;

      command.Parameters.AddWithValue("@id", entity.AnimalId);
      command.Parameters.AddWithValue("@Name", entity.Name);
      command.Parameters.AddWithValue("@ScientificName", entity.ScientificName);
      command.Parameters.AddWithValue("@CommonName", entity.CommonName);
      command.Parameters.AddWithValue("@Sex", (entity.Sex == "male"));
      command.Parameters.AddWithValue("@BirthDate", entity.BirthDate);
      command.Parameters.AddWithValue("@Status", entity.Status);
      command.Parameters.AddWithValue("@LocationId", long.Parse(entity.LocationId));

      await command.ExecuteNonQueryAsync();
    }

    public async Task ToggleAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        UPDATE animal
        SET IsDeleted = CASE WHEN IsDeleted = 1 THEN 0 ELSE 1 END
        WHERE Animal_ID = @id;
      """;

      command.Parameters.AddWithValue("@id", id);

      await command.ExecuteNonQueryAsync();
    }
  }
}
