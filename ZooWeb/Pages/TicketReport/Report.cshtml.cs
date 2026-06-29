using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ZooWeb.Pages.TicketReport
{
    public class ReportModel : PageModel
    {
        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

        public List<TicketSaleInfo> TicketSaleInfo { get; set; } = new List<TicketSaleInfo>();
        public int TotalTickets { get; set; }

        public void OnGet()
        {
 
        }

        public IActionResult OnPost()
        {
            try
            {
                string passType = Request.Form["PassType"];
                string employeeId = Request.Form["EmployeeID"];

                Console.WriteLine($"PassType: {passType}, EmployeeID: {employeeId}");

                string connectionString = "Server=tcp:zoowebdb.database.windows.net,1433;Database=ZooWeb_db;User ID=zooadmin;Password=peanuts420!;Trusted_Connection=False;Encrypt=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Ticket_ID, Pass_type, Eid, Visitor_pn, R_date, R_total FROM ticket_sales WHERE (@PassType IS NULL OR Pass_type = @PassType) AND (@EmployeeID IS NULL OR Eid = @EmployeeID)";

                    Console.WriteLine($"SQL Query: {sql}");

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PassType", string.IsNullOrEmpty(passType) ? DBNull.Value : (object)passType);
                        command.Parameters.AddWithValue("@EmployeeID", string.IsNullOrEmpty(employeeId) ? DBNull.Value : (object)employeeId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TicketSaleInfo info = new TicketSaleInfo();
                                info.TicketID = reader.IsDBNull(0) ? string.Empty : reader.GetInt32(0).ToString();
                                info.PassType = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                info.EmployeeID = reader.IsDBNull(2) ? string.Empty : reader.GetInt32(2).ToString();
                                info.VisitorPn = reader.IsDBNull(3) ? string.Empty : reader.GetInt64(3).ToString();
                                info.SaleDate = reader.IsDBNull(4) ? string.Empty : reader.GetDateTime(4).ToString();
                                info.SaleTotal = reader.IsDBNull(5) ? string.Empty : reader.GetSqlMoney(5).ToString();

                                TicketSaleInfo.Add(info);
                            }
                        }
                    }

                    TotalTickets = TicketSaleInfo.Count;

                    if (TotalTickets == 0)
                    {
                        ErrorMessage = "No records found for the specified Pass Type or Employee ID.";
                    }

                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

                if (ex.Message == "String '' was not recognized as a valid DateTime.")
                {
                    ErrorMessage = "Dates cannot be empty.";
                }
                else
                {
                    ErrorMessage = ex.Message;
                }

                return Page();
            }
        }
    }

    public class TicketSaleInfo
    {
        public string TicketID { get; set; }
        public string PassType { get; set; }
        public string EmployeeID { get; set; }
        public string VisitorPn { get; set; }
        public string SaleDate { get; set; }
        public string SaleTotal { get; set; }
    }
}
