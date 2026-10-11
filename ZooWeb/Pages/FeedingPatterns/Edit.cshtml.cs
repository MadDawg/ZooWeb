using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.FeedingPatterns
{
  public class EditModel : PageModel
  {
    public FeedingPatternInfo info = new FeedingPatternInfo();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly FeedingPatternRepository _repo;

    public EditModel(FeedingPatternRepository repo)
    {
      _repo = repo;
    }

    public async Task <IActionResult> OnGet()
    {
      String Animal_Id = Request.Query["id"];
      // TODO: actually use this (addresses race condition)
      if (Animal_Id == null || Animal_Id == "") { errorMsg = "y tho?"; return Page(); };

      info = await _repo.GetByIdAsync(int.Parse(Animal_Id));
      return Page();
    }

    public async Task <IActionResult> OnPost()
    {
      //must add check for null later
      info.AnimalId = Request.Form["AnimalId"];
      info.Meal = Request.Form["Meal"];
      info.Portion = Request.Form["Portion"];
      string[] selectedDays = Request.Form["ScheduleDays"];
      string scheduleDays = string.Join(",", selectedDays);
      info.ScheduleTime = TimeSpan.Parse(Request.Form["ScheduleTime"]);

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

      foreach (FieldInfo field in fields)
      {
        if (field.Name == "Schedule_days") continue; // Skip validation for ScheduleDays

        object fieldValue = field.GetValue(info);
        if (fieldValue == null || (fieldValue is string && string.IsNullOrWhiteSpace((string)fieldValue)))
        {
          errorMsg = "All fields are required";
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
        if (field.FieldType == typeof(List<string>))
        {
          field.SetValue(info, new List<string>()); // Initialize a new empty list
        }
        else if (field.FieldType == typeof(TimeSpan))
        {
          field.SetValue(info, TimeSpan.Zero); // Set TimeSpan fields to TimeSpan.Zero
        }
        else
        {
          field.SetValue(info, ""); // Set other fields to empty string
        }
      }
      successMsg = "Feeding Pattern Updated";

      return RedirectToPage("/FeedingPatterns/Index");
    }
  }
}
