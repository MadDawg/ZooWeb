using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Pages.Visitor
{
	[Authorize(Roles = "admin, zookeeper")]
	public class IndexModel : PageModel
	{
		public List<VisitorInfo> listVisitors = new List<VisitorInfo>();
    
    private readonly VisitorRepository _repo;

    public IndexModel(VisitorRepository repo){
      _repo = repo;
    }
        
		public async Task<IActionResult> OnGet()
		{
      listVisitors = await _repo.GetAllAsync();
      return Page();
		}
	}
}
