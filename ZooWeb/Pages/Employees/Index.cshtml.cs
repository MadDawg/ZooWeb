using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Employees
{
  [Authorize(Policy = "admin")]
  public class IndexModel : PageModel
  {
    public List<EmployeeInfo> listEmployees = new List<EmployeeInfo>();
    
    private readonly EmployeeRepository _employeeRepo;

    public IndexModel(EmployeeRepository employeeRepo)
    {
      _employeeRepo = employeeRepo;
    }

    public async Task<IActionResult> OnGet()
    {
      listEmployees = await _employeeRepo.GetAllAsync();
      return Page();
    }
    public async Task<IActionResult> OnPostToggleStatus(int id){
      await _employeeRepo.ToggleAsync(id);
      return RedirectToPage();
    }
  }
}  
