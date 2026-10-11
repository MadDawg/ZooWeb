using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Enclosures
{
  [Authorize(Roles = "admin, zookeeper")]
  public class IndexModel : PageModel
  {
    public List<EnclosureInfo> listEnclosures = new List<EnclosureInfo>();
    
    private readonly EnclosureRepository _repo;

    public IndexModel(EnclosureRepository repo)
    {
      _repo = repo;
    }
    
    public async Task<IActionResult> OnGet()
    {
      listEnclosures = await _repo.GetAllAsync();
      return Page();
    }
    public async Task<IActionResult> OnPostDelete(int id){
      await _repo.DeleteByIdAsync(id);
      return RedirectToPage();
    }
  }
}
