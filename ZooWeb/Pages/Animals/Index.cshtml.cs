using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Numerics;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Animals
{
  [Authorize(Roles = "admin, zookeeper")]
  public class IndexModel : PageModel
  {
    public List<AnimalInfo> listAnimals = new List<AnimalInfo>();

    private readonly AnimalRepository _animalRepo;

    public IndexModel(AnimalRepository animalRepo){
      _animalRepo = animalRepo;
    }

    public async Task<IActionResult> OnGet()
    {
      listAnimals = await _animalRepo.GetAllAsync();
      return Page();
    }
    public async Task<IActionResult> OnPostToggleStatus(int id){
      await _animalRepo.ToggleAsync(id);
      return RedirectToPage();
    }
  }
}

