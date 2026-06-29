using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace ZooWeb.Pages.Enclosures
{
	public class EditModel : PageModel
	{
		public EnclosureInfo info = new EnclosureInfo();
		public string errorMsg = "";
		public string successMsg = "";
		public void OnGet()
		{
			String LocationId = Request.Query["id"];
			// TODO: actually use this (addresses race condition)
			if (LocationId == null || LocationId == "") { errorMsg = "y tho?"; return; };

			string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				String sql = "SELECT * FROM Enclosure WHERE LocationID=@LocationId";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@LocationId", LocationId);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							info.LocationID = reader.GetInt64(0).ToString();
							info.Type = reader.GetString(1);
							info.Capacity = reader.GetInt32(2).ToString();
							info.Occupant_Num = reader.GetInt32(3).ToString();
						}
					}
				}
			}
		}
		public void OnPost()
		{
			//must add check for null later
			info.LocationID = Request.Form["LocationId"];
			info.Type = Request.Form["Type"];
			info.Capacity = Request.Form["Capacity"];
			info.Occupant_Num = Request.Form["OccupantNum"];

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
				string connectionString = "Server=tcp:zoowebdbserver.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "UPDATE Enclosure " +
						"SET Type=@Type, Capacity=@Capacity, Occupant_Num=@OccupantNum " +
						"WHERE LocationID=@LocationId";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@LocationId", info.LocationID);
						command.Parameters.AddWithValue("@Type", info.Type);
						command.Parameters.AddWithValue("@Capacity", info.Capacity);
						command.Parameters.AddWithValue("@OccupantNum", info.Occupant_Num);

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
			successMsg = "Enclosure Updated";

			Response.Redirect("/Enclosures/Index");
		}
	}
}
