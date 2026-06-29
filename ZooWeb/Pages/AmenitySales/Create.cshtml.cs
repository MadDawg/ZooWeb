using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;

namespace ZooWeb.Pages.AmenitySales
{
	public class CreateModel : PageModel
	{
		public AmenitytSalesInfo info = new AmenitytSalesInfo();
		public string errorMsg = "";
		public string successMsg = "";
		public void OnGet()
		{
		}

		public void OnPost()
		{

			info.EID = Request.Form["EID"];
			info.LocationID = Request.Form["LocationID"];
			info.SaleType = Request.Form["SaleType"];
			//info.SaleDate = Request.Form["SaleDate"];
			info.SaleTotal = Request.Form["Total"];
			//info.SaleId = Request.Form["ReceiptNumber"];

			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			string[] excludedNames = { "SaleDate", "SaleId" };

			foreach (FieldInfo field in fields)
			{
				object fieldValue = field.GetValue(info);
				if (!excludedNames.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
				{
					errorMsg = "All fields are required";
					return;
				}
			}

			try
			{
				string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();

					string sql = "INSERT INTO amenitySales (Eid, LocationID, SaleType, SaleTotal)" +
						"	VALUES (@EID, @LocationID, @SaleType, @SaleTotal)";
					
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@EID", info.EID);
						command.Parameters.AddWithValue("@LocationID", info.LocationID);
						command.Parameters.AddWithValue("@SaleType", info.SaleType);
						command.Parameters.AddWithValue("@SaleTotal", info.SaleTotal);

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
			successMsg = "New Amenity Sale Added";

			Response.Redirect("/AmenitySales/Index");
		}
	}
}
