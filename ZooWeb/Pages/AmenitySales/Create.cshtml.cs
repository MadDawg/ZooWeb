using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;
using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Pages.AmenitySales
{
  public class CreateModel : PageModel
  {
    public AmenitySaleInfo info = new AmenitySaleInfo();
    public List<ListTable> employeeList = new List<ListTable>();

    public string errorMsg = "";
    public string successMsg = "";

    private readonly AmenitySaleRepository _amenitySaleRepo;
    private readonly EmployeeRepository _employeeRepo;

    public CreateModel(AmenitySaleRepository amenitySaleRepo, EmployeeRepository employeeRepo){
      _amenitySaleRepo = amenitySaleRepo;
      _employeeRepo = employeeRepo;
    }

    public async Task<IActionResult> OnGet()
    {
      employeeList = await _employeeRepo.GetAllMinimalAsync();
      return Page();
    }

    public async Task<IActionResult> OnPost()
    {
      AmenitySaleInfo formInfo = new AmenitySaleInfo();
      formInfo.EmployeeId = Request.Form["EID"];
      formInfo.LocationId = Request.Form["LocationID"];
      formInfo.SaleType = Request.Form["SaleType"];
      //formInfo.SaleDate = Request.Form["SaleDate"];
      formInfo.SaleTotal = Request.Form["Total"];
      //formInfo.SaleId = Request.Form["ReceiptNumber"];

      FieldInfo[] fields = formInfo.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
      string[] excludedNames = { "SaleDate", "SaleId", "IsValid" };

      foreach (FieldInfo field in fields)
      {
        object fieldValue = field.GetValue(formInfo);
        if (!excludedNames.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
        {
          errorMsg = "Missing field: " + field.Name;
          return Page();
        }
      }

      try
      {
        await _amenitySaleRepo.AddAsync(formInfo);
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        return Page();
      }


      foreach (FieldInfo field in fields)
      {
        field.SetValue(formInfo, "");
      }
      successMsg = "New Amenity Sale Added";

      return RedirectToPage("/AmenitySales/Index");
    }
  }
}
