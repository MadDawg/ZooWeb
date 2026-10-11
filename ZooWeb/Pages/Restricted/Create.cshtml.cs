using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using ZooWeb.Pages.FeedingPatterns;
using ZooWeb.Data;
using ZooWeb.Data.Entities;


namespace ZooWeb.Pages.Restricted
{
  public class CreateModel : PageModel
  {
    public RestrictedInfo info = new RestrictedInfo();
    public List<ListTable> enclosureList = new List<ListTable>();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly RestrictedRepository _restrictedRepo;
    private readonly EnclosureRepository _enclosureRepo;

    public CreateModel(RestrictedRepository restrictedRepo, EnclosureRepository enclosureRepo){
      _restrictedRepo = restrictedRepo;
      _enclosureRepo = enclosureRepo;
    }

    public async Task<IActionResult> OnGet()
    {
      enclosureList = await _enclosureRepo.GetAllMinimalAsync();
      return Page();
    }

    public async Task<IActionResult> OnPost() 
    {
      //must add check for null later

      info.LocationId = Request.Form["Location_ID"];
      info.CloseDate = DateTime.Parse(Request.Form["Close_date"]);
      info.ReopenDate = DateTime.Parse(Request.Form["Reopen_date"]);

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

      foreach (FieldInfo field in fields)
      {
        object fieldValue = field.GetValue(info);
        if (fieldValue == "" || fieldValue == null)
        {
          errorMsg = "Missing required field: " + field.Name;
          return Page();
        }
      }

      try
      {
        await _restrictedRepo.AddAsync(info);
        /*using (SqlConnection connection = _factory.CreateConnection())
        {
          connection.Open();
          string sql = "INSERT INTO restricted VALUES (@Location_ID, @Close_date, @Reopen_date)";
          using (SqlCommand command = new SqlCommand(sql, connection))
          {
            command.Parameters.AddWithValue("@Location_ID", info.LocationId);

            // Validate and format Close_date
            if (!DateTime.TryParse(info.CloseDate.ToString(), out DateTime closeDate))
            {
              errorMsg = "Invalid Close date format. Please use the format yyyy-MM-dd HH:mm:ss.";
            }
            else
            {
              command.Parameters.AddWithValue("@Close_date", closeDate);
            }


            // Validate and format Reopen_date
            if (DateTime.TryParse(info.ReopenDate.ToString(), out DateTime reopenDate))
            {
              command.Parameters.AddWithValue("@Reopen_date", reopenDate);
            }
            else
            {
              errorMsg = "Invalid Reopen date format. Please use the format yyyy-MM-dd HH:mm:ss.";

            }


            command.ExecuteNonQuery();
          }
        }*/
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        return Page();
      }
      /*
      foreach (FieldInfo field in fields)
      {
        field.SetValue(info, "");
      }*/
      successMsg = "New Restriction Added";

      return RedirectToPage("/Restricted/Index");
    }
  }
}
