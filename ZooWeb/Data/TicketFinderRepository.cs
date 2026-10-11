
using Microsoft.Data.SqlClient;
// using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class TicketFinderRepository{

    private readonly IDbConnectionFactory _factory;

    public TicketFinderRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }
    
    public async Task<List<TicketFinderInfo>> GetAllByIdAsync(long id){
      List<TicketFinderInfo> tickets = new List<TicketFinderInfo>();
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();
      await using var command = connection.CreateCommand();

      command.CommandText = """
        SELECT 
          visitor.FirstName AS FirstName,
          visitor.LastName AS LastName,
          visitor.PhoneNumber AS PhoneNumber,
          ticket_sales.Ticket_Id AS TicketId,
          ticket_sales.Pass_type AS PassType,
          ticket_sales.Eid AS EmployeeId,
          ticket_sales.R_date AS SaleDate,
          ticket_sales.R_total AS SaleTotal,
          ticket_sales.IsValid AS IsValid
        FROM visitor
        INNER JOIN ticket_sales
          ON visitor.PhoneNumber = ticket_sales.Visitor_Pn
        WHERE visitor.PhoneNumber = @id
        """;

      command.Parameters.AddWithValue("@id", id);
      
      await using var reader = await command.ExecuteReaderAsync();

      while(await reader.ReadAsync()){
        TicketFinderInfo entity = new TicketFinderInfo{
          FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
          LastName = reader.GetString(reader.GetOrdinal("LastName")),
          PhoneNumber = reader.GetInt64(reader.GetOrdinal("PhoneNumber")).ToString(),
          TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")).ToString(),
          PassType = reader.GetString(reader.GetOrdinal("PassType")),
          EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")).ToString(),
          SaleDate = reader.GetDateTime(reader.GetOrdinal("SaleDate")).ToString(),
          SaleTotal = reader.GetSqlMoney(reader.GetOrdinal("SaleTotal")).ToString(),
          IsValid = reader.GetBoolean(reader.GetOrdinal("IsValid")) ? "valid" : "invalid"
        };
        tickets.Add(entity);
      }
      return tickets;
    }
  }
}
