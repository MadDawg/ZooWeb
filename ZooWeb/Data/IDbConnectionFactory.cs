using Microsoft.Data.SqlClient;

namespace ZooWeb.Data;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
}
