using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;

namespace ZooWeb.Pages.Department
{
	public class CreateModel : PageModel
	{
		public DepartmentInfo info = new DepartmentInfo();
		public string errorMsg = "";
		public string successMsg = "";
		public void OnGet()
		{
		}

		public void OnPost()
		{

			info.Dnumber = Request.Form["Department_number"];
			info.Name = Request.Form["Name"];

			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			string[] excludedFields = { "Dnumber" };

			foreach (FieldInfo field in fields)
			{
				object fieldValue = field.GetValue(info);
				if (!excludedFields.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
				{
					errorMsg = "Missing required field: " + field.Name;
					return;
				}
			}

			try
			{
				string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "INSERT INTO department (Name) VALUES (@Name)";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						//command.Parameters.AddWithValue("@Dnumber", int.Parse(info.Dnumber));
						command.Parameters.AddWithValue("@Name", info.Name);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				errorMsg = ex.Message;
				return;
			}


			foreach (FieldInfo field in fields)
			{
				field.SetValue(info, "");
			}
			successMsg = "New Department Added";

			Response.Redirect("/Department/Index");
		}
	}
}

