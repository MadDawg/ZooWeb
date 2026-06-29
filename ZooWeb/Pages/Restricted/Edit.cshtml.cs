using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace ZooWeb.Pages.Restricted
{
	public class EditModel : PageModel
	{

		[DataType(DataType.Date)]
		public DateTime Close_date { get; set; }
		public DateTime Reopen_date { get; set; }

		public RestrictedInfo info = new RestrictedInfo();
		public string errorMsg = "";
		public string successMsg = "";
		public void OnGet()
		{
			string locationid = Request.Query["id"];

			string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				String sql = "SELECT * FROM restricted WHERE Location_ID=@locationid";
				using (SqlCommand command = new SqlCommand(sql, connection))
                {
					command.Parameters.AddWithValue("@locationid", locationid);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
                            info.Location_ID = reader.GetInt64(0).ToString();
                            info.Close_date = reader.GetDateTime(1);
                            info.Reopen_date = reader.GetDateTime(2);
							//System.Diagnostics.Debug.WriteLine(Close_date);
						}
					}
				}
			}
		}
		public void OnPost()
		{
            //must add check for null later
            info.Location_ID = Request.Form["Location_ID"];
			info.Close_date = DateTime.Parse(Request.Form["Close_date"]);
			info.Reopen_date = DateTime.Parse(Request.Form["Reopen_date"]);

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

			try
			{
				if(info.Close_date > info.Reopen_date)
				{
					throw new Exception("Close Date cannot exceed Reopen Date.");
				}
				if (info.Reopen_date < info.Close_date)
				{
					throw new Exception("Reopen Date must come after Close Date.");
				}

				string connectionString = "Server=tcp:zoowebdbserver.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "UPDATE restricted " +
						"SET Close_date=@Close_date, Reopen_date=@Reopen_date " +
                        "WHERE Location_ID=@Location_ID";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
                        command.Parameters.AddWithValue("@Location_ID", info.Location_ID);
                        command.Parameters.AddWithValue("@Close_date", info.Close_date);
                        command.Parameters.AddWithValue("@Reopen_date", info.Reopen_date);

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
			successMsg = "Restriction Updated";

            Response.Redirect("/Restricted/Index");
		}
	}
}
