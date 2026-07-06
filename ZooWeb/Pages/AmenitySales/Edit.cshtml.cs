using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZooWeb.Pages.AmenitySales;
using Microsoft.Data.SqlClient;
using System.Reflection;
using Microsoft.CodeAnalysis;
using System.Linq;
using ZooWeb.Data;


namespace ZooWeb.Pages.AmenitySales
{
  public class EditModel : PageModel
  {
    public AmenitytSalesInfo info = new AmenitytSalesInfo();
    public string errorMsg = "";
    public string successMsg = "";

    private readonly IDbConnectionFactory _factory;

    public EditModel(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

    public void OnGet()
    {
      /*String Location_ID = Request.Query["id"];
        if (Location_ID == null || Location_ID == "") { errorMsg = "y tho?"; return; };*/
      String SaleId = Request.Query["id"];

      using (SqlConnection connection = _factory.CreateConnection())
      {
        connection.Open();
        String sql = "SELECT * " +
          "FROM amenitySales " +
          "WHERE SaleId=@SaleId";
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
          command.Parameters.AddWithValue("@SaleId", SaleId);
          using (SqlDataReader reader = command.ExecuteReader())
          {
            if (reader.Read())
            {
              info.EID = reader.GetInt32(0).ToString();
              info.LocationID = reader.GetInt32(1).ToString();
              info.SaleType = reader.GetString(2);
              info.SaleDate = reader.GetDateTime(5).ToString("yyyy-MM-dd");
              info.SaleTotal = reader.GetDecimal(4).ToString();
              info.SaleId = reader.GetInt64(3).ToString();
            }
          }
        }
      }
    }
    public void OnPost()
    {


      info.EID = Request.Form["EID"];
      info.LocationID = Request.Form["LocationID"];
      info.SaleType = Request.Form["SaleType"];
      //info.SaleDate = Request.Form["SaleDate"];
      info.SaleTotal = Request.Form["Total"];
      info.SaleId = Request.Form["SaleId"];

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

          string sql = "UPDATE amenitySales " +
            "SET Eid=@EID, SaleType=@SaleType, LocationID=@LocationID " +
            "WHERE SaleId=@SaleId";

          using (SqlCommand command = new SqlCommand(sql, connection))
          {
            command.Parameters.AddWithValue("@Eid", info.EID);
            command.Parameters.AddWithValue("@LocationID", info.LocationID);
            command.Parameters.AddWithValue("@SaleType", info.SaleType);
            command.Parameters.AddWithValue("@SaleId", info.SaleId);

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
      successMsg = "Amenity Sale Updated";

      Response.Redirect("/AmenitySales/Index");
    }
  }
}
