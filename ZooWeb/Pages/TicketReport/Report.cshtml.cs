using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.TicketReport
{
  public class ReportModel : PageModel
  {
    public string ErrorMessage { get; set; } = "";
    public string SuccessMessage { get; set; } = "";

    public List<TicketSaleInfo> listTicketSale = new List<TicketSaleInfo>();

    private readonly TicketSaleRepository _repo;

    public ReportModel(TicketSaleRepository repo){
      _repo = repo;
    }

    public async Task<IActionResult> OnGet()
    {
      return Page();
    }

    public async Task<IActionResult> OnPost()
    {
      string passType = Request.Form["PassType"];
      string employeeId = Request.Form["EmployeeID"];
      // Console.WriteLine($"PassType: {passType}, EmployeeID: {employeeId}");
      try
      {
        listTicketSale = await _repo.GetAllFilteredAsync(passType, employeeId);

        if (!listTicketSale.Any())
        {
          ErrorMessage = "No records found for the specified Pass Type or Employee ID.";
        }
      }
      catch (Exception ex)
      {
        // Console.WriteLine($"Exception: {ex.Message}");

        if (ex.Message == "String '' was not recognized as a valid DateTime.")
        {
          ErrorMessage = "Dates cannot be empty.";
        }
        else
        {
          ErrorMessage = ex.Message;
        }
      }
      return Page();
    }
  }
}
