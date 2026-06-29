using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Isopoh.Cryptography.Argon2;
using Microsoft.Data.SqlClient;
using ZooWeb.Pages.ZooUsers;
using System.Data;


namespace ZooWeb.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Role;

        public async Task<IActionResult> OnPostAsync()
        {
			// Validate username and password
			if (IsValidUser(Username, Password))
            {
                //indicate the user is authenticated.
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Username),
                    new Claim("user", Role),
                    new Claim(ClaimTypes.Role, Role),
                    // Add other claims as needed
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                // Redirect to the main page if login is successful
                return RedirectToPage("/Home/Index");
            }
            else
            {
                //if login fails
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }

        private bool IsValidUser(string username, string password)
        {
			string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";
			ZooUserInfo info = new ZooUserInfo();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				String sql = "SELECT UserID, Username, PasswordHash, UserRole, IsActive "
					+ "FROM zoo_user "
					+ "WHERE Username = @Username";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@Username", username);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							//info.UserId = reader.GetInt32(0).ToString();
							info.Username = reader.GetString(1);
							info.PasswordHash = reader.GetString(2);
                            try
                            {
                                Role = reader.GetString(3);
                            }
                            catch (Exception ex)
                            {
                                return false;
							}
                            if (!reader.GetBoolean(4)) {
                                return false;
                            }
                        }
					}
				}
			}
			//validate the user here.
            // TODO: validate against roles table when it is created
			if (!String.IsNullOrEmpty(info.Username) && !String.IsNullOrEmpty(info.PasswordHash) && Argon2.Verify(info.PasswordHash, password))
            {
                return true;
            }

            return false;
        }
    }
}
