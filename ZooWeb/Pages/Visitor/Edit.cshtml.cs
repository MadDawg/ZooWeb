using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Data;


namespace ZooWeb.Pages.Visitor
{
	public class EditModel : PageModel
	{
		public VisitorInfo info = new VisitorInfo();
		public string errorMsg = "";
		public string successMsg = "";

    private readonly IDbConnectionFactory _factory;

    public EditModel(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

		public void OnGet()
		{
			String PhoneNumber = Request.Query["id"];
			// TODO: actually use this (addresses race condition)
			if (PhoneNumber == null || PhoneNumber == "") { errorMsg = "y tho?"; return; };

			using (SqlConnection connection = _factory.CreateConnection())
			{
				connection.Open();
				String sql = "SELECT * FROM visitor WHERE PhoneNumber=@PhoneNumber";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
                            info.PhoneNumber = reader.GetInt64(0).ToString();
                            info.FirstName = reader.GetString(1);
                            info.LastName = reader.GetString(2);
                            info.BirthDate = reader.GetDateTime(3).ToString();

                        }
					}
				}
			}
		}
		public void OnPost()
		{
            //must add check for null later
            info.PhoneNumber = Request.Query["id"];
			//System.Diagnostics.Debug.WriteLine(info.PhoneNumber);
            info.FirstName = Request.Form["FirstName"];
			info.LastName = Request.Form["LastName"];
			info.BirthDate = Request.Form["BirthDate"];

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
			  using (SqlConnection connection = _factory.CreateConnection())
				{
					connection.Open();
                    string sql = "UPDATE visitor " +
             "SET FirstName=@FirstName, LastName=@LastName, BirthDate=@BirthDate " +
             "WHERE PhoneNumber=@PhoneNumber";

                    using (SqlCommand command = new SqlCommand(sql, connection))
					{
                        command.Parameters.AddWithValue("@PhoneNumber", info.PhoneNumber);
                        command.Parameters.AddWithValue("@FirstName", info.FirstName);
                        command.Parameters.AddWithValue("@LastName", info.LastName);
                        command.Parameters.AddWithValue("@BirthDate", info.BirthDate);

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
			successMsg = "Visitor updated";

			Response.Redirect("/Visitor/Index");
		}
	}
}
