using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.AmenitySales
{
  [Authorize(Policy = "admin")]
  public class IndexModel : PageModel
  {
    public List<AmenitySaleInfo> amenitySales = new List<AmenitySaleInfo>();
    
    private readonly AmenitySaleRepository _amenitySaleRepo;

    public IndexModel(AmenitySaleRepository amenitySaleRepo){
      _amenitySaleRepo = amenitySaleRepo;
    }

    public async Task<IActionResult> OnGet()
    {
      amenitySales = await _amenitySaleRepo.GetAllAsync();
      return Page();
    }
    public async Task<IActionResult> OnPostToggleStatus(int id){
      await _amenitySaleRepo.ToggleAsync(id);
      return RedirectToPage();
    }
  }
}
