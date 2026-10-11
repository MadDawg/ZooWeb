using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZooWeb.Pages.AmenitySales;
using Microsoft.Data.SqlClient;
using System.Reflection;
using Microsoft.CodeAnalysis;
using System.Linq;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.AmenitySales
{
  public class EditModel : PageModel
  {
    public AmenitySaleInfo? info = new AmenitySaleInfo();
    public List<ListTable> employeeList = new List<ListTable>();
    public string errorMsg = "";
    public string successMsg = "";
    
    private readonly AmenitySaleRepository _amenitySaleRepo;
    private readonly EmployeeRepository _employeeRepo;

    public EditModel(AmenitySaleRepository amenitySaleRepo, EmployeeRepository employeeRepo){
      _amenitySaleRepo = amenitySaleRepo;
      _employeeRepo = employeeRepo;
    }

    public async Task<IActionResult> OnGet()
    {
      String saleId = Request.Query["id"];
      info = await _amenitySaleRepo.GetByIdAsync(int.Parse(saleId));
      employeeList = await _employeeRepo.GetAllMinimalAsync();
      return Page();
    }

    public async Task<IActionResult> OnPost()
    {
      info.EmployeeId = Request.Form["EID"];
      info.LocationId = Request.Form["LocationID"];
      info.SaleType = Request.Form["SaleType"];
      //info.SaleDate = Request.Form["SaleDate"];
      info.SaleTotal = Request.Form["Total"];
      info.SaleId = Request.Form["SaleId"];

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
      string[] excludedNames = { "SaleDate", "SaleId", "IsValid" };
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
        await _amenitySaleRepo.UpdateAsync(info);
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
      successMsg = "Amenity Sale Updated";

      return RedirectToPage("/AmenitySales/Index");
    }
  }
}
