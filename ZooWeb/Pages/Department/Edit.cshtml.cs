using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZooWeb.Pages.Department;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Department
{
  public class EditModel : PageModel
  {
    public DepartmentInfo info = new DepartmentInfo();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly DepartmentRepository _repo;

    public EditModel(DepartmentRepository repo){
      _repo = repo;
    }

    public async Task<IActionResult> OnGet()
    {
      String Department_number = Request.Query["id"];
      if (Department_number == null || Department_number == "") { errorMsg = "y tho?"; return Page(); };

      info = await _repo.GetByIdAsync(short.Parse(Department_number));
      return Page();
    }

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
        await _repo.UpdateAsync(info);
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
      successMsg = "Department Updated";

      return RedirectToPage("/Department/Index");
    }
  }
}
