using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class TicketSaleRepository{

    private readonly IDbConnectionFactory _factory;

    public TicketSaleRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }
    
    public async Task<TicketSaleInfo?> GetByIdAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        SELECT *
        FROM ticket_sales
        WHERE Visitor_Pn = @id
        """;

      command.Parameters.AddWithValue("@id", id);
      
      await using var reader = await command.ExecuteReaderAsync();
      if (!await reader.ReadAsync()){ return null; }

      return new TicketSaleInfo{
        TicketId = reader.GetInt32(reader.GetOrdinal("Ticket_Id")).ToString(),
        PassType = reader.GetString(reader.GetOrdinal("Pass_type")),
        EmployeeId = reader.GetInt32(reader.GetOrdinal("Eid")).ToString(),
        VisitorPhoneNumber = reader.GetInt64(reader.GetOrdinal("Visitor_Pn")).ToString(),
        SaleDate = reader.GetDateTime(reader.GetOrdinal("R_date")).ToString(),
        SaleTotal = reader.GetSqlMoney(reader.GetOrdinal("R_total")).ToString(),
        IsValid = reader.GetBoolean(reader.GetOrdinal("IsValid")) ? "valid" : "invalid"
      };
    }

    public async Task<List<TicketSaleInfo>> GetAllAsync(){
      List<TicketSaleInfo> ticket_sales = new List<TicketSaleInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        SELECT *
        FROM ticket_sales
        """;

      await using var reader = await command.ExecuteReaderAsync();
  
      while (await reader.ReadAsync()){
        TicketSaleInfo entity = new TicketSaleInfo{
          TicketId = reader.GetInt32(reader.GetOrdinal("Ticket_Id")).ToString(),
          PassType = reader.GetString(reader.GetOrdinal("Pass_type")),
          EmployeeId = reader.GetInt32(reader.GetOrdinal("Eid")).ToString(),
          VisitorPhoneNumber = reader.GetInt64(reader.GetOrdinal("Visitor_Pn")).ToString(),
          SaleDate = reader.GetDateTime(reader.GetOrdinal("R_date")).ToString(),
          SaleTotal = reader.GetSqlMoney(reader.GetOrdinal("R_total")).ToString(),
          IsValid = reader.GetBoolean(reader.GetOrdinal("IsValid")) ? "valid" : "invalid"
        };
        ticket_sales.Add(entity);
      }
      return ticket_sales;
    }

    public async Task<List<TicketSaleInfo>> GetAllFilteredAsync(string? passType, string? employeeId){
      List<TicketSaleInfo> ticket_sales = new List<TicketSaleInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        SELECT Ticket_Id, Pass_type, Eid, Visitor_Pn, R_date, R_total
        FROM ticket_sales
        WHERE IsValid = 1
        """;

      if (!string.IsNullOrEmpty(passType) && !string.IsNullOrEmpty(employeeId)){
        command.CommandText += " AND Pass_type = @PassType AND Eid = @EmployeeId";
        command.Parameters.AddWithValue("@PassType", passType);
        command.Parameters.AddWithValue("@EmployeeId", employeeId);
      }
      else if(!string.IsNullOrEmpty(passType)){
        command.CommandText += " AND Pass_type = @PassType";
        command.Parameters.AddWithValue("@PassType", passType);
      }
      else if (!string.IsNullOrEmpty(employeeId)){
        command.CommandText += " AND Eid = @EmployeeId";
        command.Parameters.AddWithValue("@EmployeeId", employeeId);
      }


      await using var reader = await command.ExecuteReaderAsync();
  
      while (await reader.ReadAsync()){
        TicketSaleInfo entity = new TicketSaleInfo{
          TicketId = reader.GetInt32(reader.GetOrdinal("Ticket_Id")).ToString(),
          PassType = reader.GetString(reader.GetOrdinal("Pass_type")),
          EmployeeId = reader.GetInt32(reader.GetOrdinal("Eid")).ToString(),
          VisitorPhoneNumber = reader.GetInt64(reader.GetOrdinal("Visitor_Pn")).ToString(),
          SaleDate = reader.GetDateTime(reader.GetOrdinal("R_date")).ToString(),
          SaleTotal = reader.GetSqlMoney(reader.GetOrdinal("R_total")).ToString()
        };
        ticket_sales.Add(entity);
      }
      return ticket_sales;
    }
    
    public async Task AddAsync(TicketSaleInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();
      
      command.CommandText = """
        INSERT INTO ticket_sales (Pass_type, Eid, Visitor_Pn, R_date, R_total)
        VALUES (@PassType, @EmployeeId, @VisitorPhoneNumber, @SaleDate, @SaleTotal)
        """;

      command.Parameters.AddWithValue("@PassType", entity.PassType);
      command.Parameters.AddWithValue("@EmployeeId", entity.EmployeeId);
      command.Parameters.AddWithValue("@VisitorPhoneNumber", entity.VisitorPhoneNumber);
      command.Parameters.AddWithValue("@SaleDate", entity.SaleDate);
      command.Parameters.AddWithValue("@SaleTotal", entity.SaleTotal);

      await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(TicketSaleInfo entity){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        UPDATE ticket_sales
        SET Pass_type=@PassType, Eid=@EmployeeId, Visitor_Pn=@VisitorPhoneNumber, R_date=@SaleDate, R_total=@SaleTotal)
        WHERE Ticket_Id=@id
        """;

      command.Parameters.AddWithValue("@id", entity.TicketId);
      command.Parameters.AddWithValue("@PassType", entity.PassType);
      command.Parameters.AddWithValue("@EmployeeId", entity.EmployeeId);
      command.Parameters.AddWithValue("@VisitorPhoneNumber", entity.VisitorPhoneNumber);
      command.Parameters.AddWithValue("@SaleDate", entity.SaleDate);
      command.Parameters.AddWithValue("@SaleTotal", entity.SaleTotal);
      
      await command.ExecuteNonQueryAsync();
    }
    
    public async Task ToggleAsync(int id){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        UPDATE ticket_sales
        SET IsValid = CASE WHEN IsValid = 1 THEN 0 ELSE 1 END
        WHERE Ticket_Id = @id;
        """;

      command.Parameters.AddWithValue("@id", id);
      
      await command.ExecuteNonQueryAsync();
    }
  }
}
