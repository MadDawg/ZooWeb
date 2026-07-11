using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;
using ZooWeb.Data;


namespace ZooWeb.Pages.Department
{
	public class CreateModel : PageModel
	{
		public DepartmentInfo info = new DepartmentInfo();
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

			info.Dnumber = Request.Form["Department_number"];
			info.Name = Request.Form["Name"];

			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			string[] excludedFields = { "Dnumber" };

			foreach (FieldInfo field in fields)
			{
				object fieldValue = field.GetValue(info);
				if (!excludedFields.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
				{
					errorMsg = "Missing required field: " + field.Name;
					return;
				}
			}

			try
			{
				using (SqlConnection connection = _factory.CreateConnection())
				{
					connection.Open();
					string sql = "INSERT INTO department (Name) VALUES (@Name)";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						//command.Parameters.AddWithValue("@Dnumber", int.Parse(info.Dnumber));
						command.Parameters.AddWithValue("@Name", info.Name);

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
			successMsg = "New Department Added";

			Response.Redirect("/Department/Index");
		}
	}
}

