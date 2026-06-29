using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;


namespace ZooWeb.Pages.Department
{
	[Authorize(Policy = "admin")]
	public class IndexModel : PageModel
	{
		public List<DepartmentInfo> ListDepartment = new List<DepartmentInfo>();

		public void OnGet()
		{

			string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

			using (SqlConnection connection = new SqlConnection(connectionString))
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
	}

}
public class DepartmentInfo
{
	public string Dnumber;
	public string Name;
}
