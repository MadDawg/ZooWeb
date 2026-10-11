using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Animals
{
  public class EditModel : PageModel
  {
    public AnimalInfo info = new AnimalInfo();
    public List<ListTable> enclosureList = new List<ListTable>();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly AnimalRepository _animalRepo;
    private readonly EnclosureRepository _enclosureRepo;

    public EditModel(AnimalRepository animalRepo, EnclosureRepository enclosureRepo){
      _animalRepo = animalRepo;
      _enclosureRepo = enclosureRepo;
    }

    public async Task<IActionResult> OnGet()
    {
      String Animal_Id = Request.Query["id"];
      // TODO: actually use this (addresses race condition) // wait does it?
      if (Animal_Id == null || Animal_Id == "") { errorMsg = "y tho?"; return Page(); };

      info = await _animalRepo.GetByIdAsync(int.Parse(Animal_Id));
      enclosureList = await _enclosureRepo.GetAllMinimalAsync();
      return Page();
    }
    public async Task<IActionResult> OnPost()
    {
      //must add check for null later
      info.AnimalId = Request.Form["Animal_Id"];
      info.Name = Request.Form["Name"];
      info.ScientificName = Request.Form["Scientific_name"];
      info.CommonName = Request.Form["Common_name"];
      info.Sex = Request.Form["Sex"];
      info.BirthDate = DateTime.Parse(Request.Form["Birth_date"]);
      info.Status = Request.Form["Status"];
      info.LocationId = Request.Form["Location_Id"];

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

      foreach (FieldInfo field in fields)
      {
        object fieldValue = field.GetValue(info);
        if ((fieldValue == "" || fieldValue == null))
        {
          errorMsg = "All fields are required";
          return Page();
        }
      }
      if (!info.Sex.Equals("female", StringComparison.OrdinalIgnoreCase) && !info.Sex.Equals("male", StringComparison.OrdinalIgnoreCase))
      {
        errorMsg = "Animal's sex must be male or female.";
        return Page();
      }

      try
      {
        await _animalRepo.UpdateAsync(info);
        // return Page();
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        return Page();
      }

      foreach (FieldInfo field in fields)
      {
        if (field.FieldType == typeof(DateTime))
        {
          // For DateTime fields, set them to DateTime.MinValue to clear the value.
          field.SetValue(info, DateTime.MinValue);
        }
        else
        {
          // For other fields (e.g., string fields), set them to an empty string.
          field.SetValue(info, "");
        }
      }
      successMsg = "Animal Updated";

      return RedirectToPage("/Animals/Index");
    }
  }
}
