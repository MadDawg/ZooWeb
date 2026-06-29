using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Pages.Animals;

namespace ZooWeb.Pages.FeedingPatterns
{
	public class CreateModel : PageModel
	{
		public FeedingPatternInfo info = new FeedingPatternInfo();
		public List<AnimalListTable> animalList = new List<AnimalListTable>();
		public string errorMsg = "";
		public string successMsg = "";
		public void OnGet()
		{
			string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				String sql = "SELECT Animal_ID, Name, Common_name "
					+ "FROM animal";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							animalList.Add(new AnimalListTable
							{
								Key = reader.GetInt32(0).ToString(),
								Display = reader.GetString(1) + " (" + reader.GetString(2) + ")"
							});
						}
					}
				}
			}
		}

		public void OnPost()
		{
			//must add check for null later
			info.Animal_ID = Request.Form["AnimalId"];
			info.Meal = Request.Form["Meal"];
			info.Portion = Request.Form["Portion"];
            string[] selectedDays = Request.Form["ScheduleDays"];
            string scheduleDays = string.Join(",", selectedDays);
            info.Schedule_time = TimeSpan.Parse(Request.Form["ScheduleTime"]);

            FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                if (field.Name == "Schedule_days") continue; // Skip validation for ScheduleDays

                object fieldValue = field.GetValue(info);
                if (fieldValue == null || (fieldValue is string && string.IsNullOrWhiteSpace((string)fieldValue)))
                {
                    errorMsg = "All fields are required";
                    return;
                }
            }

            try
			{
				string connectionString = "Server=tcp:zoowebdbserver.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "INSERT INTO Feeding_Pattern VALUES (@AnimalId, @Meal, @Portion, @ScheduleDays, @ScheduleTime)";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@AnimalId", info.Animal_ID);
						command.Parameters.AddWithValue("@Meal", info.Meal);
						command.Parameters.AddWithValue("@Portion", info.Portion);
						command.Parameters.AddWithValue("@ScheduleDays", scheduleDays);
						command.Parameters.AddWithValue("@ScheduleTime", info.Schedule_time);

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
                if (field.FieldType == typeof(List<string>))
                {
                    field.SetValue(info, new List<string>()); // Initialize a new empty list
                }
				else if (field.FieldType == typeof(TimeSpan))
				{
					field.SetValue(info, TimeSpan.Zero); // Set TimeSpan fields to TimeSpan.Zero
				}
				else
                {
                    field.SetValue(info, ""); // Set other fields to empty string
                }
            }
            successMsg = "New FeedingPattern Added";

			Response.Redirect("/FeedingPatterns/Index");
		}
	}
	public class AnimalListTable
	{
		public string Key { get; set; }
		public string Display { get; set; }
	}
}
