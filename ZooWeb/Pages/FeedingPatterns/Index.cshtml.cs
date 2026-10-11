using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using System.Diagnostics;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.FeedingPatterns
{
  [Authorize(Roles = "admin, zookeeper")]
  public class IndexModel : PageModel
  {
    public List<FeedingPatternInfo> listFeedingPatterns = new List<FeedingPatternInfo>();

    private readonly FeedingPatternRepository _repo;

    public IndexModel(FeedingPatternRepository repo)
    {
      _repo = repo;
    }

    public async Task <IActionResult> OnGet()
    {
      listFeedingPatterns = await _repo.GetAllAsync();
      return Page();
    }

    public async Task<IActionResult> OnPostDelete(int id){
      await _repo.DeleteByIdAsync(id);
      return RedirectToPage();
    }
  }
}
