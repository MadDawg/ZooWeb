using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ZooWeb.Pages.Revenue;
using Microsoft.Data.SqlClient;
using System.Data;
using ZooWeb.Data;


namespace ZooWeb.Pages.FeedingReport
{
	public class ReportModel : PageModel
	{
		public string errorMsg = "";
		public string successMsg = "";
    
    private readonly IDbConnectionFactory _factory;

    public ReportModel(IDbConnectionFactory factory)
    {
      _factory = factory;
    }


		[DataType(DataType.Date)]
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		public void OnGet()
		{
		}

		public List<animalInfo> animalInfo = new List<animalInfo>();
		public void OnPost()
		{
			try
			{
				string AnimalId = Request.Form["aid"];

				if (startDate > endDate)
				{
					throw new Exception("Start date cannot exceed end date.");
				}

				using (SqlConnection connection = _factory.CreateConnection())
				{
					connection.Open();
					string sql = "SELECT animal.Animal_ID, Name, Meal, Portion, Schedule_days, Schedule_time, Status," +
                        " Location_ID FROM feeding_pattern, animal WHERE animal.Animal_Id = @aid AND feeding_pattern.Animal_Id = @aid";


					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@aid", AnimalId);

						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								animalInfo info = new animalInfo();
								info.AnimalId = reader.GetInt32(0).ToString();
								info.Name = reader.GetString(1);
								info.Meal = reader.GetString(2);
								info.Portion = reader.GetDecimal(3).ToString();
								info.Schedule_days = reader.GetString(4);
								info.Schedule_time = reader.GetTimeSpan(5).ToString();
								info.Status = reader.GetString(6);
								info.Location = reader.GetInt64(7).ToString();

								animalInfo.Add(info);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.Message == "String '' was not recognized as a valid DateTime.")
				{
					errorMsg = "Dates cannot be empty.";
				}
				else
				{
					errorMsg = ex.Message;
				}
			}
		}
	}

	public class animalInfo
	{
		public string AnimalId;
		public string Name;
		public string Meal;
		public string Portion;
		public string Schedule_days;
		public string Schedule_time;
		public string Status;
		public string Location;
	}
}
