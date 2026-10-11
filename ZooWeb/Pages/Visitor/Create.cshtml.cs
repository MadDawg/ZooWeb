using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Visitor
{
  public class CreateModel : PageModel
  {
    public VisitorInfo info = new VisitorInfo();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly VisitorRepository _repo;

    public CreateModel(VisitorRepository repo){
      _repo = repo;
    }

    public async Task<IActionResult> OnGet()
    {
      return Page();
    }

    public async Task<IActionResult> OnPost() 
    {
      //must add check for null later
      info.PhoneNumber = Request.Form["PhoneNumber"];
      info.FirstName = Request.Form["FirstName"];
      info.LastName = Request.Form["LastName"];
      info.BirthDate = Request.Form["BirthDate"];

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

      foreach (FieldInfo field in fields)
      {
        object fieldValue = field.GetValue(info);
        if (fieldValue == "" || fieldValue == null)
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
        field.SetValue(info, "");
      }
      successMsg = "New Visitor Added";

      return RedirectToPage("/Visitor/Index");
    }
  }
}
