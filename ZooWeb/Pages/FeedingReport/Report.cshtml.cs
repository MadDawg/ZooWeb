using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ZooWeb.Pages.Revenue;
using Microsoft.Data.SqlClient;
using System.Data;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.FeedingReport
{
	public class ReportModel : PageModel
	{
    public List<ListTable> animalList = new List<ListTable>();
		public List<FeedingReportInfo> reportInfoList = new List<FeedingReportInfo>();
		public string errorMsg = "";
		public string successMsg = "";
    
    private readonly FeedingReportRepository _feedingReportRepo;
    private readonly AnimalRepository _animalRepo;

    public ReportModel(FeedingReportRepository feedingReportRepo, AnimalRepository animalRepo)
    {
      _animalRepo = animalRepo;
      _feedingReportRepo = feedingReportRepo;
    }

		[DataType(DataType.Date)]
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		
    public async Task <IActionResult> OnGet()
		{
      animalList = await _animalRepo.GetAllMinimalAsync();
      return Page();
		}

		public async Task <IActionResult> OnPost()
		{
			try
			{
				string AnimalId = Request.Form["aid"];

				if (startDate > endDate)
				{
					throw new Exception("Start date cannot exceed end date.");
				}

        reportInfoList = await _feedingReportRepo.GetByIdAsync(int.Parse(AnimalId));
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
			return Page();
		}
	}
}
