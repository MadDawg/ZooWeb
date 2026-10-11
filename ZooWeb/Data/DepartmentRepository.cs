using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class DepartmentRepository{

    private readonly IDbConnectionFactory _factory;

    public DepartmentRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public async Task<DepartmentInfo?> GetByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM department
        WHERE Dnumber = @id
        """;

      command.Parameters.AddWithValue("@id", id);

      await using var reader = await command.ExecuteReaderAsync();
      if (!await reader.ReadAsync()){ return null; }

      return new DepartmentInfo{
        DepartmentNumber = reader.GetInt16(reader.GetOrdinal("Dnumber")).ToString(),
        Name = reader.GetString(reader.GetOrdinal("Name"))
      };
    }

    public async Task<List<DepartmentInfo>> GetAllAsync(){
      List<DepartmentInfo> departments = new List<DepartmentInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM department
        """;

      await using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync()){
        DepartmentInfo entity = new DepartmentInfo{
          DepartmentNumber = reader.GetInt16(reader.GetOrdinal("Dnumber")).ToString(),
          Name = reader.GetString(reader.GetOrdinal("Name"))
        };
        departments.Add(entity);
      }
      return departments;
    }

    public async Task<List<ListTable>> GetAllAsListTableAsync(){
      List<DepartmentInfo> departments = await GetAllAsync();
      List<ListTable> departmentsTbl = new List<ListTable>();
      foreach (var department in departments){
        departmentsTbl.Add(new ListTable
            {
            Key = department.DepartmentNumber,
            Display = department.Name
            });
      }
      return departmentsTbl;
    }

    public async Task AddAsync(DepartmentInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """INSERT INTO department (Name) VALUES (@Name)""";

      command.Parameters.AddWithValue("@Name", entity.Name);

      await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(DepartmentInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        UPDATE department
        SET Name=@Name
        WHERE Dnumber=@DNumber
        """;

      command.Parameters.AddWithValue("@Name", entity.Name);
      command.Parameters.AddWithValue("@Dnumber", entity.DepartmentNumber);

      await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        DELETE department WHERE Dnumber = @id
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
        DELETE department WHERE Name = @name
        """;

      await using var reader = await command.ExecuteReaderAsync();
      command.Parameters.AddWithValue("@name", name);

      await command.ExecuteNonQueryAsync();
    }
  }
}
