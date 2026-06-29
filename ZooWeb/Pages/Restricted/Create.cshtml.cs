using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Pages.FeedingPatterns;

namespace ZooWeb.Pages.Restricted
{
    public class CreateModel : PageModel
    {
		public RestrictedInfo info = new RestrictedInfo();
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

			info.Location_ID = Request.Form["Location_ID"];
			info.Close_date = DateTime.Parse(Request.Form["Close_date"]);
			info.Reopen_date = DateTime.Parse(Request.Form["Reopen_date"]);

			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

			foreach (FieldInfo field in fields)
			{
				object fieldValue = field.GetValue(info);
				if (fieldValue == "" || fieldValue == null)
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
					string sql = "INSERT INTO restricted VALUES (@Location_ID, @Close_date, @Reopen_date)";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@Location_ID", info.Location_ID);

						// Validate and format Close_date
						if (!DateTime.TryParse(info.Close_date.ToString(), out DateTime closeDate))
						{
							errorMsg = "Invalid Close date format. Please use the format yyyy-MM-dd HH:mm:ss.";
						}
						else
						{
							command.Parameters.AddWithValue("@Close_date", closeDate);
						}


						// Validate and format Reopen_date
						if (DateTime.TryParse(info.Reopen_date.ToString(), out DateTime reopenDate))
						{
							command.Parameters.AddWithValue("@Reopen_date", reopenDate);
						}
						else
						{
							errorMsg = "Invalid Reopen date format. Please use the format yyyy-MM-dd HH:mm:ss.";

						}


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
			successMsg = "New Restriction Added";

			Response.Redirect("/Restricted/Index");
		}
    }
    public class EnclosureListTable
    {
        public string Key { get; set; }
        public string Display { get; set; }
    }
}
