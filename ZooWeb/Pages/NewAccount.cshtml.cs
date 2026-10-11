using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using ZooWeb.Pages.ZooUsers;
using Microsoft.Data.SqlClient;
using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Pages
{
  public class NewAccountModel : PageModel
  {
    public ZooProfileInfo info = new ZooProfileInfo();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly ZooProfileRepository _repo;
    
    public NewAccountModel(ZooProfileRepository repo){
      _repo = repo;
    }

    public async Task<IActionResult> OnGet()
    {
      return Page();
    }

    public async Task<IActionResult> OnPost()
    {
      info.Username = Request.Form["Username"];
      info.PasswordHash = Argon2.Hash(Request.Form["Password"]);
      info.FirstName = Request.Form["Fname"];
      info.LastName = Request.Form["Lname"];
      info.PhoneNumber = Request.Form["Pn"];
      info.BirthDate = DateTime.Parse(Request.Form["Bday"]);
      //info.IsActive = Request.Form["Status"];

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
      string[] excludedNames = {"UserId", "CreationDate", "IsActive"};

      foreach (FieldInfo field in fields)
      {
        object fieldValue = field.GetValue(info);
        if (!excludedNames.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
        {
          errorMsg = "Missing field: " + field.Name;
          return Page();
        }
      }

      try
      {
        await _repo.AddAsync(info);
      }

      catch (Exception ex)
      {
        errorMsg = ex.Message;
        return Page();
      }

      foreach (FieldInfo field in fields)
      {
        if (field.FieldType == typeof(string)) {field.SetValue(info, "");}
        else { field.SetValue(info, null); }

      }
      successMsg = "New User Added";

      return RedirectToPage("/Index");

    }
  }
}
