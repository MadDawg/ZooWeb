using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZooWeb.Pages.Department;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace ZooWeb.Pages.Department
{
    public class EditModel : PageModel
    {
		public DepartmentInfo info = new DepartmentInfo();
		public string errorMsg = "";
		public string successMsg = "";
		public void OnGet()
		{
			String Department_number = Request.Query["id"];
			if (Department_number == null || Department_number == "") { errorMsg = "y tho?"; return; };

			string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				String sql = "SELECT * FROM department WHERE Dnumber=@Department_number";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@Department_number", Department_number);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							info.Dnumber = reader.GetInt16(0).ToString();
							info.Name = reader.GetString(1);
						}
					}
				}
			}
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
				string connectionString = "Server=tcp:zoowebdbserver.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "UPDATE Department " +
						"SET Name=@Name " + 
						"WHERE Dnumber=@DNumber";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@Dnumber", info.Dnumber);
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
			successMsg = "Department Updated";

			Response.Redirect("/Department/Index");
		}
	}
}
