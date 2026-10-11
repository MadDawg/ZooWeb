using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
// using ZooWeb.Pages.Employees;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Revenue
{
  public class ReportModel : PageModel
  {
    public string errorMsg = "";
    public string successMsg = "";
    public string source = "";
    public string eid;
    public decimal revenueTotal = 0;
    
    public List<RevenueInfo> listRevenue = new List<RevenueInfo>();

    private readonly RevenueRepository _repo;

    public ReportModel(RevenueRepository repo){
      _repo = repo;
    }

    [DataType(DataType.Date)]
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public async Task<IActionResult> OnGet()
    {
      return Page();
    }

    public async Task<IActionResult> OnPost() 
    {
      try
      {
        // TODO: TryParse here to avoid checking for FormatException later
        // move this outside of try block
        // date range does not need to throw
        startDate = DateTime.Parse(Request.Form["start"]);
        endDate = DateTime.Parse(Request.Form["end"]);
        eid = Request.Form["eid"];
        source = Request.Form["src"];

        if (startDate > endDate)
        {
          throw new Exception("Start date cannot exceed end date.");
        }
        listRevenue = await _repo.GetFilteredAsync(startDate, endDate, eid, source);
        foreach(var item in listRevenue){
          revenueTotal += Convert.ToDecimal(item.Total); //TODO: just store a decimal in the data model instead...
        }
      }
      catch (Exception ex)
      {
        if (ex.Message == "String '' was not recognized as a valid DateTime.")
        {
          errorMsg = "Dates cannot be empty.";
        }
        else
        {
          errorMsg = ex.Message;
        }
      }
      return Page(); //TODO: is this what we are supposed to be returning in every instance?
    }
  }
}
