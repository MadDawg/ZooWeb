using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Restricted
{
  public class EditModel : PageModel
  {

    [DataType(DataType.Date)]
    public DateTime Close_date { get; set; }
    public DateTime Reopen_date { get; set; }

    public RestrictedInfo info = new RestrictedInfo();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly RestrictedRepository _repo;

    public EditModel(RestrictedRepository repo){
      _repo = repo;
    }

    public async Task<IActionResult> OnGet()
    {
      string locationId = Request.Query["id"];
      info = await _repo.GetByIdAsync(int.Parse(locationId));
      return Page();
    }

    public async Task<IActionResult> OnPost()
    {
      //must add check for null later
      info.LocationId = Request.Form["Location_ID"];
      info.CloseDate = DateTime.Parse(Request.Form["Close_date"]);
      info.ReopenDate = DateTime.Parse(Request.Form["Reopen_date"]);

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
        if(info.CloseDate > info.ReopenDate)
        {
          throw new Exception("Close Date cannot exceed Reopen Date.");
        }
        if (info.ReopenDate < info.CloseDate)
        {
          throw new Exception("Reopen Date must come after Close Date.");
        }

        await _repo.UpdateAsync(info);
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        return Page();
      }

      foreach (FieldInfo field in fields)
      {
        if (field.FieldType == typeof(DateTime))
        {
          // For DateTime fields, set them to DateTime.MinValue to clear the value.
          field.SetValue(info, DateTime.MinValue);
        }
        else
        {
          // For other fields (e.g., string fields), set them to an empty string.
          field.SetValue(info, "");
        }
      }
      successMsg = "Restriction Updated";

      return RedirectToPage("/Restricted/Index");
    }
  }
}
