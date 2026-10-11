using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.TicketFinder
{
  public class ReportModel : PageModel
  {
    public string errorMsg = "";
    public string successMsg = "";

    private readonly TicketFinderRepository _repo;

    public ReportModel(TicketFinderRepository repo){
      _repo = repo;
    }

    // TODO: We should be using this, but maybe not in this manner
    [BindProperty]
    public VisitorInfo Visitor { get; set; }

    public List<TicketFinderInfo> listVisitors = new List<TicketFinderInfo>();

    public async Task<IActionResult> OnGet()
    {
      return Page();
    }

    public async Task<IActionResult> OnPost()
    {
      if (!IsValidPhoneNumber(Visitor.PhoneNumber))
      {
        errorMsg = "Invalid phone number format. Please enter a valid phone number.";
        return Page();
      }
      try
      {
        listVisitors = await _repo.GetAllByIdAsync(long.Parse(Visitor.PhoneNumber));
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        return Page();
      }
      if (!listVisitors.Any())
      {
        successMsg = "Data retrieved successfully.";
        return Page();
      }
      else
      {
        errorMsg = "No records found for this phone number.";
        return Page();
      }
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
      return !string.IsNullOrEmpty(phoneNumber) && phoneNumber.All(char.IsDigit);
    }
  }
}




