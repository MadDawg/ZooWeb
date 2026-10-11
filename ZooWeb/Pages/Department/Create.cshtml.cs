using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;
using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Pages.Department
{
  public class CreateModel : PageModel
  {
    public DepartmentInfo info = new DepartmentInfo();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly DepartmentRepository _repo;

    public CreateModel(DepartmentRepository repo){
      _repo = repo;
    }

    public async Task<IActionResult> OnGet()
    {
      return Page();
    }

    // TODO: Department number seems to be unused.
    public async Task<IActionResult> OnPost()
    {
      info.DepartmentNumber = Request.Form["Department_number"];
      info.Name = Request.Form["Name"];

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
      string[] excludedFields = { "Dnumber" };

      foreach (FieldInfo field in fields)
      {
        object fieldValue = field.GetValue(info);
        if (!excludedFields.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
        {
          errorMsg = "Missing required field: " + field.Name;
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
        field.SetValue(info, "");
      }
      successMsg = "New Department Added";

      return RedirectToPage("/Department/Index");
    }
  }
}

