using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using Isopoh.Cryptography.Argon2;
using System.Data.SqlTypes;
using System.Linq;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.ZooUsers
{
  public class EditModel : PageModel
  {
    private readonly ZooUserRepository _zooUserRepo;
    private readonly ZooUserRoleRepository _zooUserRoleRepo;
    
    public List<ListTable> userRoleList = new List<ListTable>();
    public ZooUserInfo info = new ZooUserInfo();
    
    public string errorMsg = "";
    public string successMsg = "";
    
    public EditModel(ZooUserRepository zooUserRepo, ZooUserRoleRepository zooUserRoleRepo)
    {
      _zooUserRepo = zooUserRepo;
      _zooUserRoleRepo = zooUserRoleRepo;
    }

    public async Task<IActionResult> OnGet()
    {
      String UserId = Request.Query["id"];
      // TODO: actually use this (addresses race condition)
      if (UserId == null || UserId == "") { errorMsg = "y tho?"; return Page(); };

      info = await _zooUserRepo.GetByIdAsync(int.Parse(UserId)); 
      userRoleList = await _zooUserRoleRepo.GetAllAsync();

      return Page();
    }
    
    public async Task<IActionResult> OnPost()
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
          errorMsg = "Missing field: " + field.Name;
          return Page();
        }
      }

      try
      {
        await _zooUserRepo.UpdateAsync(info);
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        return Page();
      }
      // TODO: what was this for again??
      foreach (FieldInfo field in fields)
      {
        field.SetValue(info, "");
      }
      successMsg = "User Updated";

      return RedirectToPage("/ZooUsers/Index");
    }
  }
}
