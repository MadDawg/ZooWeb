using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Pages.Enclosures
{
	public class CreateModel : PageModel
	{
		public EnclosureInfo info = new EnclosureInfo();
		public string errorMsg = "";
		public string successMsg = "";
        
    private readonly EnclosureRepository _repo;

    public CreateModel(EnclosureRepository repo)
    {
      _repo = repo;
    }

		public async Task<IActionResult> OnGet()
		{
      return Page();
		}

		public async Task<IActionResult> OnPost()
		{
			info.Type = Request.Form["Type"];
			info.Capacity = Request.Form["Capacity"];
			info.OccupantNumber = Request.Form["OccupantNum"];

			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
      string[] excludedFields = { "LocationID" }; // TODO: add OccupantNum whenever we autogenerate it
      
      foreach (FieldInfo field in fields)
      {
        object fieldValue = field.GetValue(info);
        if (!excludedFields.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
        {
          errorMsg = "Missing field: " + field.Name;
          return Page();
        }
      }

			try
			{
        await _repo.AddAsync(info);
			}
			catch (Exception ex)
			{
				errorMsg = ex.Message;
				return Page();
			}

			foreach (FieldInfo field in fields)
			{
				field.SetValue(info, "");
			}
			successMsg = "New Enclosure Added";

			return RedirectToPage("/Enclosures/Index");
		}
	}
}
