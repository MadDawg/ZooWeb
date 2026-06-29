//using Azure;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZooWeb.Pages.Shared;


namespace ZooWeb.Pages
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostLogout()
        {
            Response.Cookies.Delete("access");
            HttpContext.SignOutAsync();

            return RedirectToPage("/Index");
        }
    }
}
