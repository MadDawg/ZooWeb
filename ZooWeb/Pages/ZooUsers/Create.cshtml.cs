using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;
using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Pages.ZooUsers
{
  public class CreateModel : PageModel
  {
    private readonly ZooUserRepository _zooUserRepo;
    private readonly ZooUserRoleRepository _zooUserRoleRepo;

    public ZooUserInfo info = new ZooUserInfo();
    public List<ListTable> userRoleList = new List<ListTable>();
    public string errorMsg = "";
    public string successMsg = "";

    public CreateModel(ZooUserRepository zooUserRepo, ZooUserRoleRepository zooUserRoleRepo)
    {
      _zooUserRepo = zooUserRepo;
      _zooUserRoleRepo = zooUserRoleRepo;
    }

    public async Task<IActionResult> OnGet()
    {
      userRoleList = await _zooUserRoleRepo.GetAllAsync();
      return Page();
    }

    public async Task<IActionResult> OnPost()
    {
      //must add check for null later
      info.Username = Request.Form["Username"];
      if (!String.IsNullOrEmpty(Request.Form["Password"]))
      {
        info.PasswordHash = Argon2.Hash(Request.Form["Password"]);
      }
      info.UserRole = Request.Form["Role"];

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
      string[] excludedFields = { "UserId", "CreationDate", "IsActive" };

      foreach (FieldInfo field in fields)
      {
        object fieldValue = field.GetValue(info);
        // we should have to check for status but it doesn't seem to work otherwise
        if (!excludedFields.Contains(field.Name)
            && (fieldValue == "" || fieldValue == null))
        {
          string fieldName = field.Name;
          if (fieldName == "PasswordHash") { fieldName = "Password"; }
          errorMsg = "Missing required field: " + fieldName;
          return Page();
        }
      }

      await _zooUserRepo.AddAsync(info);

      foreach (FieldInfo field in fields)
      {
        field.SetValue(info, "");
      }
      successMsg = "New User Added";

      return RedirectToPage("/ZooUsers/Index");
    }
  }
}
