using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Data;


namespace ZooWeb.Pages.Animals
{
	public class EditModel : PageModel
	{
		public AnimalInfo info = new AnimalInfo();
    public List<EnclosureListTable> enclosureList = new List<EnclosureListTable>();
    public string errorMsg = "";
		public string successMsg = "";

    private readonly IDbConnectionFactory _factory;

    public EditModel(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

		public void OnGet()
		{
			String Animal_Id = Request.Query["id"];
			// TODO: actually use this (addresses race condition)
			if (Animal_Id == null || Animal_Id == "") { errorMsg = "y tho?"; return; };

			using (SqlConnection connection = _factory.CreateConnection())
			{
				connection.Open();
				String sql = "SELECT * FROM animal WHERE Animal_ID=@Animal_Id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@Animal_Id", Animal_Id);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							//AnimalInfo info = new AnimalInfo();
							info.Animal_Id = reader.GetInt32(0).ToString();
							info.Name = reader.GetString(1);
							info.Scientific_name = reader.GetString(2);
							info.Common_name = reader.GetString(3);
							if (reader.GetBoolean(4)) { info.Sex = "male"; } else { info.Sex = "female"; }
							info.Birth_date = reader.GetDateTime(5);
							info.Status = reader.GetString(6);
							info.Location_Id = reader.GetInt64(7).ToString();
						}
					}
				}
                sql = "SELECT LocationID, Type "
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

			foreach (FieldInfo field in fields)
			{
				object fieldValue = field.GetValue(info);
				if ((fieldValue == "" || fieldValue == null))
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
			  using (SqlConnection connection = _factory.CreateConnection())
				{
					connection.Open();
					string sql = "UPDATE Animal " +
						"SET Name=@Name, Scientific_name=@Scientific_name, Common_name=@Common_name, Sex=@Sex, Birth_date=@Birth_date, Status=@Status, Location_ID=@Location_ID " +
						"WHERE Animal_ID=@Animal_Id";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@Animal_Id", info.Animal_Id);
						command.Parameters.AddWithValue("@Name", info.Name);
						command.Parameters.AddWithValue("@Scientific_name", info.Scientific_name);
						command.Parameters.AddWithValue("@Common_name", info.Common_name);
						command.Parameters.AddWithValue("@Sex", (info.Sex == "male"));
						command.Parameters.AddWithValue("@Birth_date", info.Birth_date);
						command.Parameters.AddWithValue("@Status", info.Status);
						command.Parameters.AddWithValue("@Location_ID", int.Parse(info.Location_Id));

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
			successMsg = "Animal Updated";

			Response.Redirect("/Animals/Index");
		}
	}
}
