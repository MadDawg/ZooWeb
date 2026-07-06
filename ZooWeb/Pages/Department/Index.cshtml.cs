using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;



namespace ZooWeb.Pages.Department
{
	[Authorize(Policy = "admin")]
	public class IndexModel : PageModel
	{
		public List<DepartmentInfo> ListDepartment = new List<DepartmentInfo>();
        
    private readonly IDbConnectionFactory _factory;

    public IndexModel(IDbConnectionFactory factory)
    {
      _factory = factory;
    }


		public void OnGet()
		{
      using (SqlConnection connection = _factory.CreateConnection())
			{
				connection.Open();
				String sql = "SELECT * FROM department";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							DepartmentInfo info = new DepartmentInfo();
							info.Dnumber = reader.GetInt16(0).ToString();
							info.Name = reader.GetString(1);

							ListDepartment.Add(info);
						}
					}
				}
			}
		}
    public IActionResult OnPostDelete(int id){

      using var connection = _factory.CreateConnection();
      connection.Open();

      const string sql = "DELETE restricted WHERE Dnumber=@id";

      using var command = new SqlCommand(sql, connection);
      command.Parameters.AddWithValue("@id", id);
      command.ExecuteNonQuery();

      return RedirectToPage();
    }
	}

}
public class DepartmentInfo
{
	public string Dnumber;
	public string Name;
}
