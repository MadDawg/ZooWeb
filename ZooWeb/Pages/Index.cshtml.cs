using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Isopoh.Cryptography.Argon2;
using Microsoft.Data.SqlClient;
using System.Data;
// using ZooWeb.Pages.ZooUsers;
using ZooWeb.Data;
using ZooWeb.Data.Entities;



namespace ZooWeb.Pages
{
  public class LoginModel : PageModel
  {
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string Role;

    private readonly ZooUserRepository _repo;

    public LoginModel(ZooUserRepository repo)
    {
      _repo = repo;
    }


    public async Task<IActionResult> OnPostAsync()
    {
      // Validate username and password
      if (await IsValidUser(Username, Password))
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

    private async Task<bool> IsValidUser(string username, string password)
    {
      ZooUserValidatorInfo? info = new ZooUserValidatorInfo();
      try{
        info = await _repo.GetByUsernameForValidationAsync(username);
      }
      catch
      {
        return false;
      }
      //validate the user here.
      return(
        info?.UserRole is not null && 
        !String.IsNullOrEmpty(info.PasswordHash) &&
        Argon2.Verify(info.PasswordHash, password)
      );
    }
  }
}
