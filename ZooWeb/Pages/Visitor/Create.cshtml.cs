using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Data;


namespace ZooWeb.Pages.Visitor
{
    public class CreateModel : PageModel
    {
        public VisitorInfo info = new VisitorInfo();
        public string errorMsg = "";
        public string successMsg = "";
        
        private readonly IDbConnectionFactory _factory;

        public CreateModel(IDbConnectionFactory factory)
        {
          _factory = factory;
        }

        public void OnGet()
        {
        }

        public void OnPost() 
        {
			//must add check for null later
			info.PhoneNumber = Request.Form["PhoneNumber"];
			info.FirstName = Request.Form["FirstName"];
			info.LastName = Request.Form["LastName"];
			info.BirthDate = Request.Form["BirthDate"];

			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

			foreach (FieldInfo field in fields)
			{
				object fieldValue = field.GetValue(info);
				if (fieldValue == "" || fieldValue == null)
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
					string sql = "INSERT INTO visitor VALUES (@PhoneNumber, @FirstName, @LastName, @BirthDate)";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@PhoneNumber", int.Parse(info.PhoneNumber));
						command.Parameters.AddWithValue("@FirstName", long.Parse(info.FirstName));
						command.Parameters.AddWithValue("@LastName", short.Parse(info.LastName));
						command.Parameters.AddWithValue("@BirthDate", long.Parse(info.BirthDate));
						
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
			successMsg = "New Visitor Added";

			Response.Redirect("/Visitor/Index");
		}
    }
}
