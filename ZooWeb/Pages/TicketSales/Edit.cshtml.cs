using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Linq;
using ZooWeb.Pages.Employees;
using ZooWeb.Data;


namespace ZooWeb.Pages.TicketSales
{
	public class EditModel : PageModel
	{
        public TicketSaleInfo info = new TicketSaleInfo();
        public List<ListTable> employeeList = new List<ListTable>();
        public List<ListTable> visitorPnList = new List<ListTable>();
        public string errorMsg = "";
        public string successMsg = "";

        private readonly IDbConnectionFactory _factory;

        public EditModel(IDbConnectionFactory factory)
        {
          _factory = factory;
        }

		public void OnGet()
		{
			String TicketSaleID = Request.Query["id"];
			// TODO: actually use this (addresses race condition)
			if (TicketSaleID == null || TicketSaleID == "") { errorMsg = "y tho?"; return; };

			using (SqlConnection connection = _factory.CreateConnection())
			{
				connection.Open();
				String sql = "SELECT * FROM ticket_sales WHERE Ticket_ID=@TicketId";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@TicketID", TicketSaleID);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							info.TicketID = reader.GetInt32(0).ToString();
							info.PassType = reader.GetString(1);
							info.EmployeeID = reader.GetInt32(2).ToString();
							info.VisitorPn = reader.GetInt64(3).ToString();
							info.SaleTotal = reader.GetSqlMoney(5).ToString();
							info.SaleDate = reader.GetDateTime(4).ToString();

						}
					}
				}

				sql = "SELECT EmployeeId, FName, Lname "
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
			info.TicketID = Request.Form["TicketID"];
			info.PassType = Request.Form["PassType"];
			info.EmployeeID = Request.Form["EmployeeID"];
			info.VisitorPn = Request.Form["VisitorPn"];
			info.SaleTotal = Request.Form["SaleTotal"];
			//info.SaleDate = Request.Form["ReceiptNumber"];

			FieldInfo[] fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			string[] excludedFields = { "TicketID", "SaleDate", "IsValid" };

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
					string sql = "UPDATE ticket_sales " +
						"SET Pass_type=@PassType, Eid=@EmployeeId, Visitor_pn=@VisitorPn, R_total=@SaleTotal" +
						" WHERE Ticket_Id=@TicketId";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@TicketId", info.TicketID);
						command.Parameters.AddWithValue("@PassType", info.PassType);
						command.Parameters.AddWithValue("@EmployeeId", info.EmployeeID);
						command.Parameters.AddWithValue("@VisitorPn", info.VisitorPn);
						command.Parameters.AddWithValue("@SaleTotal", info.SaleTotal);
						//command.Parameters.AddWithValue("@SaleDate", info.SaleDate);


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
			successMsg = "Ticket Sales Updated";

			Response.Redirect("/TicketSales/Index");
		}
	}
}
