using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using Isopoh.Cryptography.Argon2;
using System.Data.SqlTypes;
using System.Linq;
using ZooWeb.Data;


namespace ZooWeb.Pages.ZooUsers
{
	public class EditModel : PageModel
	{
        public List<RoleListTable> userRoleList = new List<RoleListTable>();
        public ZooUserInfo info = new ZooUserInfo();
        public string errorMsg = "";
        public string successMsg = "";

        private readonly IDbConnectionFactory _factory;

        public EditModel(IDbConnectionFactory factory)
        {
          _factory = factory;
        }

		public void OnGet()
		{
			String UserId = Request.Query["id"];
			// TODO: actually use this (addresses race condition)
			if (UserId == null || UserId == "") { errorMsg = "y tho?"; return; };

			using (SqlConnection connection = _factory.CreateConnection())
			{
				connection.Open();
				String sql = "SELECT * FROM zoo_user " +
					"WHERE UserId=@UserId";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@UserId", UserId);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							info.UserId = reader.GetInt32(0).ToString();
							info.Username = reader.GetString(1);
							info.PasswordHash = reader.GetString(2);
							if (reader.GetBoolean(3)) { info.IsActive = "enabled"; }
							else { info.IsActive = "disabled";  }							
							info.CreationDate = reader.GetDateTime(5).ToString();
							try
							{
								info.UserRole = reader.GetString(4);
							}
							catch (SqlNullValueException)
							{
                                info.UserRole = "-";
                            }
						}
					}
				}

                sql = "SELECT RoleName "
                    + "FROM zoo_user_role " +
                    "WHERE RoleName <> 'system'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
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
		}
		public void OnPost()
		{
			//must add check for null later
			info.UserId = Request.Form["UserId"];
            info.Username = Request.Form["Username"];
            if (!String.IsNullOrEmpty(Request.Form["Password"]))
			{
				info.PasswordHash = Argon2.Hash(Request.Form["Password"]);
			}
			info.IsActive = Request.Form["Status"];
			info.CreationDate = Request.Form["CreationDate"];
			info.UserRole = Request.Form["Role"];

			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			string[] excludedFields = { "UserId", "CreationDate", "PasswordHash" };

			foreach (FieldInfo field in fields)
			{
				object fieldValue = field.GetValue(info);
				if (!excludedFields.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
				{
					//errorMsg = "All fields are required";
					errorMsg = "Missing field: " + field.Name;
					return;
				}
			}

			try
			{
		  	using (SqlConnection connection = _factory.CreateConnection())
				{
					connection.Open();
					string sql = "UPDATE zoo_user " +
                        "SET Username=@Username, IsActive=@Status ";
                    if (!String.IsNullOrEmpty(info.PasswordHash)) {
                        sql +=  ", PasswordHash=@Password ";
                    }
					if (!String.IsNullOrEmpty(info.UserRole))
					{
						sql += ", UserRole=@UserRole ";
					}
                    sql += "WHERE UserId=@UserId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@UserId", info.UserId);
						command.Parameters.AddWithValue("@Username",info.Username);
						if (!String.IsNullOrEmpty(info.PasswordHash))
						{
							command.Parameters.AddWithValue("@Password", info.PasswordHash);
						}
                        if (!String.IsNullOrEmpty(info.UserRole))
						{
							command.Parameters.AddWithValue("@UserRole", info.UserRole);
						}
                        command.Parameters.AddWithValue("@Status", (info.IsActive == "enabled"));
						//command.Parameters.AddWithValue("@CreationDate", info.CreationDate);

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
			successMsg = "User Updated";

			Response.Redirect("/ZooUsers/Index");
		}
	}
}
