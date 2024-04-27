namespace Hair_Salon_Web_ASP.NET.Models
{
    public class AppointmentUserInfo
    {
        
            public int app_id { get; set; }
            public DateTime Date { get; set; }
            public string BookingTime { get; set; }
            public string? FinishTime { get; set; }
            public string? Status { get; set; }
            public int UserIDBooked { get; set; }
            public int EmployeeIDChosen { get; set; }

            public string EmployeeName { get; set; }
            public string ServiceName { get; set; }
            public string BookedUserName { get; set; }
            public string Price { get; set; }


        

    }
}
