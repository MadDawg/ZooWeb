using Microsoft.Data.SqlClient;

namespace ZooWeb.Data{

  public class SqlConnectionFactory : IDbConnectionFactory
  {
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
      _connectionString =
        configuration.GetConnectionString("DefaultConnection")!;
    }

    public SqlConnection CreateConnection()
      => new SqlConnection(_connectionString);
  }
}
