using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;
using ZooWeb.Pages.Employees;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.TicketSales
{
  public class CreateModel : PageModel
  {
    public TicketSaleInfo info = new TicketSaleInfo();
    public List<ListTable> employeeList = new List<ListTable>();
    public List<ListTable> visitorPnList = new List<ListTable>();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly TicketSaleRepository _ticketSaleRepo;
    private readonly EmployeeRepository _employeeRepo;
    private readonly VisitorRepository _visitorRepo;

    public CreateModel(TicketSaleRepository ticketSaleRepo, EmployeeRepository employeeRepo, VisitorRepository visitorRepo){
      _ticketSaleRepo = ticketSaleRepo;
      _employeeRepo = employeeRepo;
      _visitorRepo = visitorRepo;
    }

    public async Task<IActionResult> OnGet()
    {
      employeeList = await _employeeRepo.GetAllMinimalAsync();
      visitorPnList = await _visitorRepo.GetAllPhoneNumbersAsync();
      return Page();
    }

    public async Task<IActionResult> OnPost() 
    {
      //must add check for null later
      //info.TicketID = Request.Form["TicketID"];
      info.PassType = Request.Form["PassType"];
      info.EmployeeId = Request.Form["EmployeeID"];
      info.VisitorPhoneNumber = Request.Form["VisitorPn"];
      info.SaleTotal = Request.Form["Total"];
      //info.SaleDate = "placeholder";
      //Decimal total = decimal.Parse(Request.Form["Total"]);


      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
      string[] excludedFields = { "TicketID", "SaleDate", "IsValid" };

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
        await _ticketSaleRepo.AddAsync(info);
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
      successMsg = "New Ticket Added";

      return RedirectToPage("/TicketSales/Index");
    }
  }
}
