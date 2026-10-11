using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Revenue
{
  [Authorize(Policy = "admin")]
  public class IndexModel : PageModel
  {
    public List<RevenueInfo> listRevenue = new List<RevenueInfo>();

    private readonly RevenueRepository _repo;

    public IndexModel(RevenueRepository repo){
      _repo = repo;
    }

    public async Task<IActionResult> OnGet()
    {
      try
      {
        listRevenue = await _repo.GetAllAsync();
      }
      catch(Exception ex)
      {
        Console.WriteLine("Exception: " + ex.ToString());
      }
      return Page();
    }
  }
}

