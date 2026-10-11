using Microsoft.Data.SqlClient;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class RevenueRepository{

    private readonly IDbConnectionFactory _factory;

    public RevenueRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public async Task<RevenueInfo> GetByIdAsync(string id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT *
        FROM Revenue
        WHERE ReceiptNum = @id
        """;

      command.Parameters.AddWithValue("@id", id);

      await using var reader = await command.ExecuteReaderAsync();
      if (!await reader.ReadAsync()){ return null; }

      return new RevenueInfo{
        Total = reader.GetDecimal(reader.GetOrdinal("Total")).ToString(),
        ReceiptSource = reader.GetString(reader.GetOrdinal("ReceiptSource")),
        ReceiptNum = reader.GetString(reader.GetOrdinal("ReceiptNum")),
        RevenueDate = reader.GetDateTime(reader.GetOrdinal("RevenueDate")).ToString(),
        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")).ToString()
      };
    }

    public async Task<List<RevenueInfo>> GetFilteredAsync(DateTime startDate, DateTime endDate, string? employeeId, string? source){
      List<RevenueInfo> revenueList = new List<RevenueInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      command.CommandText = """SELECT * FROM Revenue WHERE RevenueDate >= @StartDate AND RevenueDate <= @EndDate""";
      
      command.Parameters.AddWithValue("@StartDate", startDate);
      command.Parameters.AddWithValue("@EndDate", endDate);
      
      if (!String.IsNullOrEmpty(employeeId))
      {
        command.CommandText += " AND EmployeeId=@employeeId";
        command.Parameters.AddWithValue("@employeeId", int.Parse(employeeId));
      }
      if (source != "Any")
      {
        command.CommandText += " AND ReceiptSource=@source";
        command.Parameters.AddWithValue("@source", source);
      }

      await using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync()){
        RevenueInfo entity = new RevenueInfo{
          Total = reader.GetDecimal(reader.GetOrdinal("Total")).ToString(),
          ReceiptSource = reader.GetString(reader.GetOrdinal("ReceiptSource")),
          ReceiptNum = reader.GetString(reader.GetOrdinal("ReceiptNum")),
          RevenueDate = reader.GetDateTime(reader.GetOrdinal("RevenueDate")).ToString(),
          EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")).ToString()
        };
        revenueList.Add(entity);
      }
      return revenueList;
    }

    public async Task<List<RevenueInfo>> GetAllAsync(){
      List<RevenueInfo> revenueList = new List<RevenueInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      command.CommandText = """SELECT * FROM Revenue""";

      await using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync()){
        RevenueInfo entity = new RevenueInfo{
          Total = reader.GetDecimal(reader.GetOrdinal("Total")).ToString(),
          ReceiptSource = reader.GetString(reader.GetOrdinal("ReceiptSource")),
          ReceiptNum = reader.GetString(reader.GetOrdinal("ReceiptNum")),
          RevenueDate = reader.GetDateTime(reader.GetOrdinal("RevenueDate")).ToString(),
          EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")).ToString()
        };
        revenueList.Add(entity);
      }
      return revenueList;
    }

    public async Task DeleteByIdAsync(string id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        DELETE Revenue WHERE ReceiptNum = @id
        """;

      await using var reader = await command.ExecuteReaderAsync();
      command.Parameters.AddWithValue("@id", id);

      await command.ExecuteNonQueryAsync();
    }
  }
}
