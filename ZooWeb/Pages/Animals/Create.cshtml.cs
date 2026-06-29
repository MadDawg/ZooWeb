using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Pages.ZooUsers;
using System.Linq;

namespace ZooWeb.Pages.Animals
{
	public class CreateModel : PageModel
	{
		public AnimalInfo info = new AnimalInfo();
		public List<EnclosureListTable> enclosureList = new List<EnclosureListTable>();
		public string errorMsg = "";
		public string successMsg = "";
		public void OnGet()
		{
			string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				String sql = "SELECT LocationID, Type "
					+ "FROM enclosure";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							enclosureList.Add(new EnclosureListTable
							{
								Key = reader.GetInt64(0).ToString(),
								Display = reader.GetString(1)
							});
						}
					}
				}
			}
		}

		public void OnPost()
		{
			//must add check for null later
			info.Animal_Id = Request.Form["Animal_Id"];
			info.Name = Request.Form["Name"];
			info.Scientific_name = Request.Form["Scientific_name"];
			info.Common_name = Request.Form["Common_name"];
			info.Sex = Request.Form["Sex"];
			info.Birth_date = DateTime.Parse(Request.Form["Birth_date"]);
			info.Status = Request.Form["Status"];
			info.Location_Id = Request.Form["Location_Id"];

			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			string[] excludedFields = { "Animal_Id" };

			foreach (FieldInfo field in fields)
			{
				object fieldValue = field.GetValue(info);
				if (!excludedFields.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
				{
					errorMsg = "All fields are required";
					return;
				}
			}
			if (!info.Sex.Equals("female", StringComparison.OrdinalIgnoreCase) && !info.Sex.Equals("male", StringComparison.OrdinalIgnoreCase))
			{
				errorMsg = "Animal's sex must be male or female.";
				return;
			}

			try
			{
				string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "INSERT INTO Animal (Name, Scientific_name, Common_name, Sex, Birth_date, Status, Location_ID) " +
						"VALUES (@Name, @Scientific_name, @Common_name, @Sex, @Birth_date, @Status, @Location_ID)";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						//command.Parameters.AddWithValue("@Animal_Id", info.Animal_Id);
						command.Parameters.AddWithValue("@Name", info.Name);
						command.Parameters.AddWithValue("@Scientific_name", info.Scientific_name);
						command.Parameters.AddWithValue("@Common_name", info.Common_name);
						command.Parameters.AddWithValue("@Sex", (info.Sex == "male"));
						command.Parameters.AddWithValue("@Birth_date", info.Birth_date);
						command.Parameters.AddWithValue("@Status", info.Status);
						command.Parameters.AddWithValue("@Location_ID", info.Location_Id);

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
				if (field.FieldType == typeof(DateTime))
				{
					// For DateTime fields, set them to DateTime.MinValue to clear the value.
					field.SetValue(info, DateTime.MinValue);
				}
				else
				{
					// For other fields (e.g., string fields), set them to an empty string.
					field.SetValue(info, "");
				}
			}
			successMsg = "New Animal Added";

			Response.Redirect("/Animals/Index");
		}
	}
	public class EnclosureListTable
	{
		public string Key { get; set; }
		public string Display { get; set; }
	}
}
