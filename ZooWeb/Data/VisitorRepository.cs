using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class VisitorRepository{

    private readonly IDbConnectionFactory _factory;

    public VisitorRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }
   
    public async Task<VisitorInfo?> GetByIdAsync(long id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM visitor
        WHERE PhoneNumber = @id
        """;
      
      command.Parameters.AddWithValue("@id", id);

      await using var reader = await command.ExecuteReaderAsync();
      if (!await reader.ReadAsync()){ return null; }

      return new VisitorInfo{ 
        PhoneNumber = reader.GetInt64(reader.GetOrdinal("PhoneNumber")).ToString(),
				FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
				LastName = reader.GetString(reader.GetOrdinal("LastName")),
				BirthDate = reader.GetDateTime(reader.GetOrdinal("BirthDate")).ToString()
      };
    }
    
    public async Task<VisitorInfo?> GetByPhoneNumberAsync(long phoneNumber){
      return await GetByIdAsync(phoneNumber);
    }

    public async Task<List<VisitorInfo>> GetAllAsync(){
      List<VisitorInfo> visitors = new List<VisitorInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM visitor
        """;
      
      await using var reader = await command.ExecuteReaderAsync();
  
      while (await reader.ReadAsync()){
        VisitorInfo entity = new VisitorInfo{
          PhoneNumber = reader.GetInt64(reader.GetOrdinal("PhoneNumber")).ToString(),
				  FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
				  LastName = reader.GetString(reader.GetOrdinal("LastName")),
				  BirthDate = reader.GetDateTime(reader.GetOrdinal("BirthDate")).ToString()
        };
        visitors.Add(entity);
      }
      return visitors;
    }

    public async Task<List<ListTable>> GetAllPhoneNumbersAsync(){
      List<ListTable> visitors = new List<ListTable>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT PhoneNumber
        FROM visitor
        """;
      
      await using var reader = await command.ExecuteReaderAsync();
 
      // TODO: what are we doing...
      while (await reader.ReadAsync()){
        ListTable entity = new ListTable{
          Key = reader.GetInt64(reader.GetOrdinal("PhoneNumber")).ToString(),
          Display = reader.GetInt64(reader.GetOrdinal("PhoneNumber")).ToString()
        };
        visitors.Add(entity);
      }
      return visitors;
    }
    
    public async Task AddAsync(VisitorInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        INSERT INTO visitor (PhoneNumber, FirstName, LastName, BirthDate)
        VALUES (@PhoneNumber, @FirstName, @LastName, @BirthDate)
        """;

      command.Parameters.AddWithValue("@PhoneNumber", entity.PhoneNumber);
      command.Parameters.AddWithValue("@FirstName", entity.FirstName);
      command.Parameters.AddWithValue("@LastName", entity.LastName);
      command.Parameters.AddWithValue("@BirthDate", entity.BirthDate);

      await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(VisitorInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        UPDATE visitor
        SET FirstName=@FirstName, LastName=@LastName, BirthDate=@BirthDate
        WHERE PhoneNumber=@PhoneNumber
        """;
      
      command.Parameters.AddWithValue("@PhoneNumber", entity.PhoneNumber);
      command.Parameters.AddWithValue("@FirstName", entity.FirstName);
      command.Parameters.AddWithValue("@LastName", entity.LastName);
      command.Parameters.AddWithValue("@BirthDate", entity.BirthDate);
      
      await command.ExecuteNonQueryAsync();
    }
    
    public async Task DeleteAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      // TODO: prefer the toggle
      /*
      command.CommandText = """
        UPDATE visitor
        SET IsValid = CASE WHEN IsValid = 1 THEN 0 ELSE 1 END
        WHERE PhoneNumber = @id
        """;
      */
      command.CommandText = """
        DELETE visitor WHERE PhoneNumber = @id
        """;

      command.Parameters.AddWithValue("@id", id);
      
      await command.ExecuteNonQueryAsync();
    }
  }
}
