using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using ZooWeb.Pages.ZooUsers;
using Microsoft.Data.SqlClient;
using ZooWeb.Data;


namespace ZooWeb.Pages
{
	public class NewAccountModel : PageModel
	{
		public ZooUserInfo info = new ZooUserInfo();
		public string errorMsg = "";
		public string successMsg = "";
        
    private readonly IDbConnectionFactory _factory;

    public IndexModel(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

		public void OnGet()
		{
		}

		public void OnPost()
		{
			info.Username = Request.Form["Username"];
			info.PasswordHash = Argon2.Hash(Request.Form["Password"]);
			info.Fname = Request.Form["Fname"];
			info.Lname = Request.Form["Lname"];
			info.PhoneNumber = Request.Form["Pn"];
			info.Bday = DateTime.Parse(Request.Form["Bday"]);
			//info.IsActive = Request.Form["Status"];

			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

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
				if (field.Name != "UserId"
					&& field.Name != "CreationDate"
					&& field.Name != "IsActive"
					&& (fieldValue == "" || fieldValue == null))
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

					string userSql = "INSERT INTO zoo_user (Username, PasswordHash, IsActive, CreationDate, UserRole) " +
									 "VALUES (@Username, @Password, @Status, @CreationDate, 'visitor');";

					using (SqlCommand userCommand = new SqlCommand(userSql, connection))
					{
						userCommand.Parameters.AddWithValue("@Username", info.Username);
						userCommand.Parameters.AddWithValue("@Password", info.PasswordHash);
						userCommand.Parameters.AddWithValue("@Status", true);
						userCommand.Parameters.AddWithValue("@CreationDate", DateTime.Now);

						userCommand.ExecuteNonQuery();
					}

					string visitorSql = "INSERT INTO visitor (PhoneNumber, FirstName, LastName, BirthDate) " +
										"VALUES (@PhoneNumber, @FirstName, @LastName, @Bday);";

					using (SqlCommand visitorCommand = new SqlCommand(visitorSql, connection))
					{
						visitorCommand.Parameters.AddWithValue("@Phonenumber", info.PhoneNumber);
						visitorCommand.Parameters.AddWithValue("@FirstName", info.Fname);
						visitorCommand.Parameters.AddWithValue("@LastName", info.Lname);
						visitorCommand.Parameters.AddWithValue("@Bday", info.Bday);

						visitorCommand.ExecuteNonQuery();
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
				if (field.FieldType == typeof(string)) {field.SetValue(info, "");}
				else { field.SetValue(info, null); }
				
			}
			successMsg = "New User Added";

			Response.Redirect("/Index");

		}
	}

	public class ZooUserInfo
	{
		public string UserId;
		public string Username;
		public string PasswordHash;
		public string IsActive;
		public string CreationDate;
		public string PhoneNumber;
		public string Fname;
		public string Lname;
		public DateTime Bday;
	}
}
