using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Employees
{
  public class CreateModel : PageModel
  {
    public EmployeeInfo info = new EmployeeInfo();
    public List<ListTable> employeeList = new List<ListTable>();
    public List<ListTable> departmentList = new List<ListTable>();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly EmployeeRepository _employeeRepo;
    private readonly DepartmentRepository _departmentRepo;

    public CreateModel(EmployeeRepository employeeRepo, DepartmentRepository departmentRepo)
    {
      _employeeRepo = employeeRepo;
      _departmentRepo = departmentRepo;
    }

    public async Task<IActionResult> OnGet()
    {
      employeeList = await _employeeRepo.GetAllMinimalAsync();
      departmentList = await _departmentRepo.GetAllAsListTableAsync();
      return Page();
    }

    public async Task<IActionResult> OnPost() 
    {
      //must add check for null later

      //info.EmployeeId = "0";
      info.PhoneNumber = Request.Form["Phone_num"];

      Regex rx = new Regex(@"^\d{10}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

      if (!rx.IsMatch(info.PhoneNumber))
      {
        errorMsg = "Phone number must be all numbers with no spaces";
        return Page();
      }

      info.DepartmentNumber = Request.Form["Dno"];
      info.SupervisorEmployeeId = Request.Form["Super_Eid"];
      info.Email = Request.Form["Email"];
      info.FirstName = Request.Form["Fname"];
      info.LastName = Request.Form["Lname"];
      info.Salary = Request.Form["Salary"];

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
      string[] excludedFields = { "EmployeeId", "Super_Eid", "IsEmployed" };
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
        await _employeeRepo.AddAsync(info);
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
      successMsg = "New Employee Added";

      return RedirectToPage("/Employees/Index");
    }
  }
}
