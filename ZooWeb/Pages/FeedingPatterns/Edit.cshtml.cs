using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Data;


namespace ZooWeb.Pages.FeedingPatterns
{
	public class EditModel : PageModel
	{
		public FeedingPatternInfo info = new FeedingPatternInfo();
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
				String sql = "SELECT * FROM feeding_pattern WHERE Animal_ID=@Animal_Id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@Animal_Id", Animal_Id);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							info.Animal_ID = reader.GetInt32(0).ToString();
							info.Meal = reader.GetString(1);
							info.Portion = reader.GetDecimal(2).ToString();
							info.Schedule_days = reader.GetString(3).Split(',').ToList();
							info.Schedule_time = reader.GetTimeSpan(4);
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
			  using (SqlConnection connection = _factory.CreateConnection())
				{
					connection.Open();
					string sql = "UPDATE feeding_pattern " +
						"SET Meal=@Meal, Portion=@Portion, Schedule_days=@ScheduleDays, Schedule_time=@ScheduleTime " +
						"WHERE Animal_ID=@AnimalId";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@AnimalId", int.Parse(info.Animal_ID));
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
            successMsg = "Feeding Pattern Updated";

			Response.Redirect("/FeedingPatterns/Index");
		}
	}
}
