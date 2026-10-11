using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Pages.ZooUsers
{
  public class IndexModel : PageModel
  {
    private readonly ZooUserRepository _repo;
    public string errorMsg = "";
    public List<ZooUserInfo> listZooUsers = new List<ZooUserInfo>();

    public IndexModel(ZooUserRepository repo)
    {
      _repo = repo;
    }

    public async Task<IActionResult> OnGet(){
      listZooUsers = await _repo.GetAllAsync();
      return Page();
    }

    public async Task<IActionResult> OnPostToggleStatus(int id){
      try{
        await _repo.ToggleAsync(id);
      }
      catch (SqlException ex){
        errorMsg = "At least one admin user must remain.";
        listZooUsers = await _repo.GetAllAsync();
        return Page();
      }

      return RedirectToPage();
    }
  }
}
