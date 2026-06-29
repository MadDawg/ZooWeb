using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;


namespace ZooWeb.Pages.ZooUsers
{
	public class CreateModel : PageModel
	{
		//public SelectList DropDownList { get; set; }

		public ZooUserInfo info = new ZooUserInfo();
		public List<RoleListTable> userRoleList = new List<RoleListTable>();
		public string errorMsg = "";
		public string successMsg = "";
		public void OnGet()
		{
			string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				String sql = "SELECT RoleName "
					+ "FROM zoo_user_role " +
                    "WHERE RoleName <> 'system'";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while(reader.Read())
						{
							string roleName = reader.GetString(0);
							userRoleList.Add(new RoleListTable
							{
								Key = roleName,
								Display = roleName
							});
						}
					}
				}
			}
			//DropDownList = new SelectList(userRoleList, "Key", "Display");
		}

		public void OnPost()
		{
			//must add check for null later
			info.Username = Request.Form["Username"];
			if (!String.IsNullOrEmpty(Request.Form["Password"]))
			{
				info.PasswordHash = Argon2.Hash(Request.Form["Password"]);
			}
			info.UserRole = Request.Form["Role"];
			//info.IsActive = Request.Form["Status"];

			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			string[] excludedFields = { "UserId", "CreationDate", "IsActive" };

			foreach (FieldInfo field in fields)
			{
				object fieldValue = field.GetValue(info);
				/*String acct_status = Request.Form["Status"];
				if (acct_status != "disabled" && acct_status != "enabled")
				{
					errorMsg = "Account Status must be \"enabled\" or \"disabled\", not whatever \"" + acct_status + "\" is...";
					return;
				}*/
				// we should have to check for status but it doesn't seem to work otherwise
				if (!excludedFields.Contains(field.Name)
					&& (fieldValue == "" || fieldValue == null))
				{
					string fieldName = field.Name;
					if (fieldName == "PasswordHash") { fieldName = "Password"; }
					errorMsg = "Missing required field: " + fieldName;
					return;
				}
			}

			try
			{
				string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "INSERT INTO zoo_user (Username, PasswordHash, IsActive, UserRole)" +
						"VALUES (@Username, @Password, @Status, @UserRole)";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@Username", info.Username);
						command.Parameters.AddWithValue("@Password", info.PasswordHash);
						//command.Parameters.AddWithValue("@Status", (info.IsActive == "enabled"));
						command.Parameters.AddWithValue("@Status", true);
						command.Parameters.AddWithValue("@UserRole", info.UserRole);

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
			successMsg = "New User Added";

			Response.Redirect("/ZooUsers/Index");
		}
	}
	public class RoleListTable
	{
		public string Key { get; set; }
		public string Display { get; set; }
	}
}
