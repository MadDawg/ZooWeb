using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.TicketSales
{
  [Authorize(Policy = "admin")]
  public class IndexModel : PageModel
  {
    public List<TicketSaleInfo> listTicketSales = new List<TicketSaleInfo>();

    private readonly TicketSaleRepository _repo;

    public IndexModel(TicketSaleRepository repo){
      _repo = repo;
    }

    public async Task<IActionResult> OnGet()
    {
      listTicketSales = await _repo.GetAllAsync();
      return Page();
    }

    public async Task<IActionResult> OnPostToggleStatus(int id){
      await _repo.ToggleAsync(id);
      return RedirectToPage();
    }
  }
}
