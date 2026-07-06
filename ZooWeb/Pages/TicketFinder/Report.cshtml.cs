using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using ZooWeb.Data;


namespace ZooWeb.Pages.TicketFinder
{
    public class ReportModel : PageModel
    {
        public string errorMsg = "";
        public string successMsg = "";
        
        private readonly IDbConnectionFactory _factory;

        public ReportModel(IDbConnectionFactory factory)
        {
          _factory = factory;
        }


        [BindProperty]
        public VisitorInfo Visitor { get; set; }

        public List<VisitorInfo> listVisitors = new List<VisitorInfo>();
        public List<TicketSaleInfo> ListTicketSales = new List<TicketSaleInfo>();

        public void OnGet()
        {
        }

        public void OnPost()
        {
            try
            {
                if (!IsValidPhoneNumber(Visitor.PhoneNumber))
                {
                    errorMsg = "Invalid phone number format. Please enter a valid phone number.";
                    return;
                }

                using (SqlConnection connection = _factory.CreateConnection())
                {
                    connection.Open();
                    string sql = "SELECT FirstName, LastName, PhoneNumber FROM Visitor";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VisitorInfo info = new VisitorInfo();
                                info.FirstName = reader.GetString(0);
                                info.LastName = reader.GetString(1);
                                info.PhoneNumber = reader.GetInt64(2).ToString();

                                listVisitors.Add(info);
                            }
                        }
                    }
                }

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    string sql = "SELECT * FROM ticket_sales WHERE Visitor_Pn = @PhoneNumber";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PhoneNumber", Visitor.PhoneNumber);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TicketSaleInfo info = new TicketSaleInfo();
                                info.TicketID = reader.GetInt32(0).ToString();
                                info.PassType = reader.GetString(1);
                                info.EmployeeID = reader.GetInt32(2).ToString();
                                info.VisitorPn = reader.GetInt64(3).ToString();
                                info.Date = reader.GetDateTime(4).ToString("yyyy-MM-dd");
                                info.Total = reader.GetDecimal(5).ToString();
                                info.ReceiptNumber = reader.GetInt64(6).ToString();

                                ListTicketSales.Add(info);
                            }
                        }
                    }
                }

                if (DoesDataMatchDatabase(Visitor))
                {
                    successMsg = "Data retrieved successfully.";
                }
                else
                {
                    errorMsg = "Entered data does not match any records in the database. Please re-enter the data.";
                    listVisitors.Clear();
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
        }

        private bool DoesDataMatchDatabase(VisitorInfo enteredData)
        {
            return listVisitors.Any(dbRecord =>
                dbRecord.FirstName == enteredData.FirstName &&
                dbRecord.LastName == enteredData.LastName &&
                dbRecord.PhoneNumber == enteredData.PhoneNumber);
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrEmpty(phoneNumber) && phoneNumber.All(char.IsDigit);
        }
    }

    public class VisitorInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class TicketSaleInfo
    {
        public string TicketID;
        public string PassType;
        public string EmployeeID;
        public string VisitorPn;
        public string Date;
        public string Total;
        public string ReceiptNumber;
    }
}




