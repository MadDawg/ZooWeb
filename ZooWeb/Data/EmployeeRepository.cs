using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class EmployeeRepository{

    private readonly IDbConnectionFactory _factory;

    public EmployeeRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }
    
    public async Task<EmployeeInfo?> GetByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM employee
        WHERE EmployeeId = @id
        """;
      
      command.Parameters.AddWithValue("@id", id);
      
      await using var reader = await command.ExecuteReaderAsync();
      if (!await reader.ReadAsync()){ return null; }

      return new EmployeeInfo{
        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")).ToString(),
        PhoneNumber = reader.GetString(reader.GetOrdinal("Phone_num")),
        DepartmentNumber = reader.GetInt16(reader.GetOrdinal("Dno")).ToString(),
        SupervisorEmployeeId = reader.IsDBNull(reader.GetOrdinal("Super_Eid")) ? "NULL" : reader.GetInt32(reader.GetOrdinal("Super_Eid")).ToString(),
        Email = reader.GetString(reader.GetOrdinal("Email")),
        FirstName = reader.GetString(reader.GetOrdinal("FName")),
        LastName = reader.GetString(reader.GetOrdinal("Lname")),
        Salary = reader.GetInt32(reader.GetOrdinal("Salary")).ToString(),
        IsEmployed = reader.GetBoolean(reader.GetOrdinal("IsEmployed")) ? "employed" : "not employed"
      };
    }

    public async Task<List<EmployeeInfo>> GetAllAsync(){
      List<EmployeeInfo> employees = new List<EmployeeInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM employee
        """;
      
      await using var reader = await command.ExecuteReaderAsync();
      
      while (await reader.ReadAsync()){
        EmployeeInfo entity = new EmployeeInfo{
          EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")).ToString(),
          PhoneNumber = reader.GetString(reader.GetOrdinal("Phone_num")),
          DepartmentNumber = reader.GetInt16(reader.GetOrdinal("Dno")).ToString(),
          SupervisorEmployeeId = reader.IsDBNull(reader.GetOrdinal("Super_Eid")) ? "NULL" : reader.GetInt32(reader.GetOrdinal("Super_Eid")).ToString(), // Hmmm...
          Email = reader.GetString(reader.GetOrdinal("Email")),
          FirstName = reader.GetString(reader.GetOrdinal("FName")),
          LastName = reader.GetString(reader.GetOrdinal("Lname")),
          Salary = reader.GetInt32(reader.GetOrdinal("Salary")).ToString(),
          IsEmployed = reader.GetBoolean(reader.GetOrdinal("IsEmployed")) ? "employed" : "not employed"
        };
        employees.Add(entity); 
      }
      return employees;
    }

    public async Task<List<ListTable>> GetAllMinimalAsync(){
      List<ListTable> employees = new List<ListTable>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT EmployeeId, FName, Lname
        FROM employee
        """;
      
      await using var reader = await command.ExecuteReaderAsync();
      
      while (await reader.ReadAsync()){
        ListTable entity = new ListTable{
          Key = reader.GetInt32(reader.GetOrdinal("EmployeeId")).ToString(),
          Display = reader.GetString(reader.GetOrdinal("Lname")) + ", " + reader.GetString(reader.GetOrdinal("FName"))
        };
        employees.Add(entity); 
      }
      return employees;
    }
    
    public async Task AddAsync(EmployeeInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        INSERT INTO employee (Phone_num, Dno, Super_Eid, Email, Fname, Lname, Salary)"
        VALUES (@PhoneNumber, @DepartmentNumber, @SupervisorEmployeeId, @Email, @FirstName, @LastName, @Salary)"
      """; 

      command.Parameters.AddWithValue("@PhoneNumber", entity.PhoneNumber);
      command.Parameters.AddWithValue("@DepartmentNumber", entity.DepartmentNumber);
      command.Parameters.AddWithValue("@SupervisorEmployeeId", entity.SupervisorEmployeeId);
      command.Parameters.AddWithValue("@Email", entity.Email);
      command.Parameters.AddWithValue("@FirstName", entity.FirstName);
      command.Parameters.AddWithValue("@LastName", entity.LastName);
      command.Parameters.AddWithValue("@Salary", entity.Salary);

      await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(EmployeeInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        UPDATE employee
        SET Phone_num=@PhoneNumber, Dno=@DepartmentNumber, Super_Eid=@SupervisorEmployeeId, Email=@Email, Fname=@FirstName, Lname=@LastName, Salary=@Salary
        WHERE EmployeeId=@EmployeeId"
        """;
      
      command.Parameters.AddWithValue("@EmployeeId", entity.EmployeeId);
      command.Parameters.AddWithValue("@PhoneNumber", entity.PhoneNumber);
      command.Parameters.AddWithValue("@DepartmentNumber", entity.DepartmentNumber);
      command.Parameters.AddWithValue("@SupervisorEmployeeId", entity.SupervisorEmployeeId);
      command.Parameters.AddWithValue("@Email", entity.Email);
      command.Parameters.AddWithValue("@FirstName", entity.FirstName);
      command.Parameters.AddWithValue("@LastName", entity.LastName);
      command.Parameters.AddWithValue("@Salary", entity.Salary);
      
      await command.ExecuteNonQueryAsync();
    }
    
    public async Task ToggleAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        UPDATE employee
        SET IsEmployed = CASE WHEN IsEmployed = 1 THEN 0 ELSE 1 END
        WHERE EmployeeId = @id;
        """;

      await using var reader = await command.ExecuteReaderAsync();
      command.Parameters.AddWithValue("@id", id);

      await command.ExecuteNonQueryAsync();
    }
  }
}
