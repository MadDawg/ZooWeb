using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Department
{
  [Authorize(Policy = "admin")]
  public class IndexModel : PageModel
  {
    public List<DepartmentInfo> ListDepartment = new List<DepartmentInfo>();

    private readonly DepartmentRepository _repo;

    public IndexModel(DepartmentRepository repo){
      _repo = repo;
    }

    public async Task<IActionResult> OnGet()
    {
      ListDepartment = await _repo.GetAllAsync();
      return Page();
    }
    public async Task<IActionResult> OnPostDelete(int id){
      await _repo.DeleteByIdAsync(id);
      return RedirectToPage();
    }
  }

}
