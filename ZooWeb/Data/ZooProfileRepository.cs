using Microsoft.Data.SqlClient;
using ZooWeb.Data.Entities;

namespace ZooWeb.Data{
  public class ZooProfileRepository{
    private readonly IDbConnectionFactory _factory;

    public ZooProfileRepository(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public async Task<ZooProfileInfo?> GetByIdAsync(int id){ return null; }

    public async Task<ZooProfileInfo?> GetByPhoneNumberAsync(long phoneNumber){ return null; }

    public async Task AddAsync(ZooProfileInfo info){
      using var connection = _factory.CreateConnection();
      await connection.OpenAsync();

      using var transaction = await connection.BeginTransactionAsync();
      
      try{
        await using var userCommand = connection.CreateCommand();
        userCommand.Transaction = (SqlTransaction)transaction; // TODO: check this to ensure we don't blow anything up

        userCommand.CommandText = """
          INSERT INTO zoo_user (Username, PasswordHash, IsActive, UserRole)
          OUTPUT INSERTED.ZooUserId
          VALUES (@Username, @Password, @Status, @UserRole)
          """;
        
        userCommand.Parameters.AddWithValue("@Username", info.Username);
        userCommand.Parameters.AddWithValue("@Password", info.PasswordHash);
        userCommand.Parameters.AddWithValue("@Status", true); // defaults to false on the database side
        userCommand.Parameters.AddWithValue("@UserRole", "visitor");

        var result = await userCommand.ExecuteScalarAsync();
        int zooUserId = Convert.ToInt32(result);

        await using var visitorCommand = connection.CreateCommand();
        visitorCommand.Transaction = (SqlTransaction)transaction; // TODO: ditto

        visitorCommand.CommandText = """
          INSERT INTO Visitor (UserId, PhoneNumber, FirstName, LastName, BirthDate)
          VALUES (@ZooUserId, @PhoneNumber, @FirstName, @LastName, @BirthDate)
          """;
      
        visitorCommand.Parameters.AddWithValue("@ZooUserId", zooUserId);
        visitorCommand.Parameters.AddWithValue("@PhoneNumber", info.PhoneNumber);
        visitorCommand.Parameters.AddWithValue("@FirstName", info.FirstName);
        visitorCommand.Parameters.AddWithValue("@LastName", info.LastName);
        visitorCommand.Parameters.AddWithValue("@BirthDate", info.BirthDate);
       
        await visitorCommand.ExecuteNonQueryAsync();

        await transaction.CommitAsync();
      }
      catch{
        await transaction.RollbackAsync();
        throw;
      }
    }
  }
}
