using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Enclosures
{
  public class EditModel : PageModel
  {
    public EnclosureInfo info = new EnclosureInfo();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly EnclosureRepository _repo;

    public EditModel(EnclosureRepository repo)
    {
      _repo = repo;
    }

    public async Task<IActionResult> OnGet()
    {
      String LocationId = Request.Query["id"];
      // TODO: actually use this (addresses race condition)
      if (LocationId == null || LocationId == "") { errorMsg = "y tho?"; return Page(); };

      info = await _repo.GetByIdAsync(int.Parse(LocationId));
      return Page();
    }
    public async Task<IActionResult> OnPost()
    {
      //must add check for null later
      info.LocationId = Request.Form["LocationId"];
      info.Type = Request.Form["Type"];
      info.Capacity = Request.Form["Capacity"];
      info.OccupantNumber = Request.Form["OccupantNum"];

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

      foreach (FieldInfo field in fields)
      {
        object fieldValue = field.GetValue(info);
        if ((fieldValue == "" || fieldValue == null))
        {
          errorMsg = "All fields are required";
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
      successMsg = "Enclosure Updated";

      return RedirectToPage("/Enclosures/Index");
    }
  }
}
