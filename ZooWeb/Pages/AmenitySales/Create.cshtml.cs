using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;
using ZooWeb.Data;


namespace ZooWeb.Pages.AmenitySales
{
  public class CreateModel : PageModel
  {
    public AmenitytSalesInfo info = new AmenitytSalesInfo();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly IDbConnectionFactory _factory;

    public CreateModel(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public void OnGet()
    {
    }

    public void OnPost()
    {

      info.EID = Request.Form["EID"];
      info.LocationID = Request.Form["LocationID"];
      info.SaleType = Request.Form["SaleType"];
      //info.SaleDate = Request.Form["SaleDate"];
      info.SaleTotal = Request.Form["Total"];
      //info.SaleId = Request.Form["ReceiptNumber"];

      FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
      string[] excludedNames = { "SaleDate", "SaleId" };

      foreach (FieldInfo field in fields)
      {
        object fieldValue = field.GetValue(info);
        if (!excludedNames.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
        {
          errorMsg = "All fields are required";
          return;
        }
      }

      try
      {
        using (SqlConnection connection = _factory.CreateConnection())
        {
          connection.Open();

          string sql = "INSERT INTO amenitySales (Eid, LocationID, SaleType, SaleTotal)" +
            "	VALUES (@EID, @LocationID, @SaleType, @SaleTotal)";

          using (SqlCommand command = new SqlCommand(sql, connection))
          {
            command.Parameters.AddWithValue("@EID", info.EID);
            command.Parameters.AddWithValue("@LocationID", info.LocationID);
            command.Parameters.AddWithValue("@SaleType", info.SaleType);
            command.Parameters.AddWithValue("@SaleTotal", info.SaleTotal);

            command.ExecuteNonQuery();
          }
        }
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        return;
      }


      foreach (FieldInfo field in fields)
      {
        field.SetValue(info, "");
      }
      successMsg = "New Amenity Sale Added";

      Response.Redirect("/AmenitySales/Index");
    }
  }
}
