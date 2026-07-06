using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;
using ZooWeb.Pages.Employees;
using ZooWeb.Data;


namespace ZooWeb.Pages.TicketSales
{
    public class CreateModel : PageModel
    {
        public TicketSaleInfo info = new TicketSaleInfo();
		public List<ListTable> employeeList = new List<ListTable>();
		public List<ListTable> visitorPnList = new List<ListTable>();
		public string errorMsg = "";
		public string successMsg = "";
    
    private readonly IDbConnectionFactory _factory;

    public CreateModel(IDbConnectionFactory factory)
    {
      _factory = factory;
    }

        public void OnGet()
        {
			using (SqlConnection connection = _factory.CreateConnection())
			{
				connection.Open();
				String sql = "SELECT EmployeeId, FName, Lname "
				+ "FROM employee";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							employeeList.Add(new ListTable
							{
								Key = reader.GetInt32(0).ToString(),
								Display = reader.GetString(2) + ", " + reader.GetString(1)
							});
						}
					}
				}
                
				sql = "SELECT PhoneNumber "
                + "FROM visitor";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            visitorPnList.Add(new ListTable
                            {
                                Key = reader.GetInt64(0).ToString(),
                                Display = reader.GetInt64(0).ToString()
                            });
                        }
                    }
                }
            }
		}

        public void OnPost() 
        {
			//must add check for null later
			//info.TicketID = Request.Form["TicketID"];
			info.PassType = Request.Form["PassType"];
			info.EmployeeID = Request.Form["EmployeeID"];
			info.VisitorPn = Request.Form["VisitorPn"];
			info.SaleTotal = Request.Form["Total"];
			//info.SaleDate = "placeholder";
			//Decimal total = decimal.Parse(Request.Form["Total"]);


			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			string[] excludedFields = { "TicketID", "SaleDate" };

			foreach (FieldInfo field in fields)
			{
				object fieldValue = field.GetValue(info);
				if (!excludedFields.Contains(field.Name) && (fieldValue == "" || fieldValue == null))
				{
					errorMsg = "Missing required field: " + field.Name;
					return;
				}
			}

			try
			{
				using (SqlConnection connection = _factory.CreateConnection())
				{
					connection.Open();
					string sql = "INSERT INTO ticket_sales (Pass_type, Eid, Visitor_pn, R_total) " +
						"VALUES (@PassType, @Eid, @VisitorPn, @SaleTotal)";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@PassType", info.PassType);
						command.Parameters.AddWithValue("@Eid", info.EmployeeID);
						command.Parameters.AddWithValue("@VisitorPn", info.VisitorPn);
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
			successMsg = "New Ticket Added";

			Response.Redirect("/TicketSales/Index");
		}
    }
	public class ListTable
	{
		public string Key { get; set; }
		public string Display { get; set; }
	}
}
