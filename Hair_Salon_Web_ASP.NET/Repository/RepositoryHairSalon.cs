using Hair_Salon_Web_ASP.NET.Models;
using System;

namespace Hair_Salon_Web_ASP.NET.Repository
{
    public class RepositoryHairSalon
    {
        private readonly ApplicationDbContext _db;
        public RepositoryHairSalon(ApplicationDbContext db)
        {
            _db = db;
        }
        public Dictionary<string,int> GetEmployeeWithNumberAppointment(DateTime startTime, DateTime endTime)
        {
            Dictionary<string, int> lstEmployee = new Dictionary<string, int>();
            List<User> userList = GetEmployee();
            foreach (User user in userList)
            {
                List<Appointment> appointments = _db.Appointments.Where(e => e.emp_id_chosen==user.user_id&& e.date >= startTime && e.date <= endTime  && e.status == "Finished").ToList();
                if(appointments.Count > 0)
                {
                    int totalHours = 0;
                    foreach (Appointment appointment in appointments)
                    {
                        Service service = _db.Services.Where(a => a.ser_id == appointment.ser_id).FirstOrDefault();
                        totalHours = totalHours + service.time_to_cut;

                    }
                    lstEmployee.Add(user.name, totalHours);
                }
                else
                {
                    lstEmployee.Add(user.name, 0);
                }      
                
            }
            return lstEmployee;
        }
        public Dictionary<string, int> GetNumberAppointMentByEachEmployee(DateTime startTime, DateTime endTime)
        {
            Dictionary<string, int> lstEmployeeWithNumber = new Dictionary<string, int>();
            List<User> userList = GetEmployee();
            foreach (User user in userList)
            {
                List<Appointment> appointments = _db.Appointments.Where(e => e.emp_id_chosen == user.user_id && e.date >= startTime && e.date <= endTime && e.status == "Finished").ToList();
                if (appointments.Count > 0)
                {
                    lstEmployeeWithNumber.Add(user.name, appointments.Count);
                }
                else
                {
                    lstEmployeeWithNumber.Add(user.name, 0);
                }

            }
            return lstEmployeeWithNumber;
        }
        public void UpdateAppointmentStatus(int app_id, string status)
        {
            Appointment obj = _db.Appointments.Find(app_id);
            if (obj != null)
            {
                obj.app_id = app_id;
                obj.status = status;
                _db.Appointments.Update(obj);
                _db.SaveChanges();
            }
        }

        public void UpdateEmployeeStatus(int user_id,List<User> employees)
        {
           User obj=employees.Where(e=>e.user_id==user_id).SingleOrDefault();
            if (obj != null)
            {
                obj.user_id = user_id;
               
                _db.Users.Update(obj);
                _db.SaveChanges();
            }
        }

        public void CreateUser(User user)
        {
            int costParameter = 12;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password, costParameter);
            user.password = hashedPassword;
            user.ConfirmPassword=null;
            _db.Add(user);
            _db.SaveChanges();
        }

        public List<User> GetEmployee()
        {
            List<User> employees = _db.Users.Where(e => e.role == "Employee").ToList();
            return employees;

        }
        public List<Appointment> GetAppointmentsByEmp_IdAndDate(DateTime dateTime,int emp_id)
        {
            List<Appointment> appointments= _db.Appointments.Where(e => e.date == dateTime && e.emp_id_chosen==emp_id && e.status == "Booking successfully").ToList();
            return appointments;
        }
        //filter the appointment of each user
        public List <AppointmentUserInfo> GetAppointmentsByID(int user_id)
        {
            List<AppointmentUserInfo> appointments = GetAppointmentUserInfoList().Where(e => e.UserIDBooked== user_id && e.Status== "Booking successfully").ToList();
            return appointments;
        }
        public List<AppointmentUserInfo> GetAppointmentsCanceled(int user_id)
        {
            List<AppointmentUserInfo> appointments =GetAppointmentUserInfoList().Where(e => e.UserIDBooked == user_id && e.Status =="Canceled").ToList();
            return appointments;
        }
        public List<AppointmentUserInfo> GetAppointmentsHistory(int user_id)
        {
            List<AppointmentUserInfo> appointments =  GetAppointmentUserInfoList().Where(e => e.UserIDBooked == user_id && e.Status == "Finished").ToList();
            return appointments;
        }

        public User GetUserByPhoneNumber(string phone_number)
        {
            User user = _db.Users.Where(u => u.phone_number==phone_number).SingleOrDefault();
            return user;
        }
        /*
        public List<AppointmentServiceName> GetOrderCustomerList()
        {
            List<AppointmentServiceName> orderCustomerList = (from appointment in _db.Appointments
                                                         join service in _db.Services
                                                         on appointment.ser_id equals service.ser_id
                                                         select new AppointmentServiceName
                                                         {
                                                             app_id = appointment.app_id,
                                                             name = service.name,
                                                             price= service.price,
                                                             time_to_cut= service.time_to_cut
                                                         }).ToList();

            return orderCustomerList;
        }

        public class AppointmentServiceName
        {
            public int app_id { get; set; }
            public string name { get; set; }
            public string price { get; set; }
            public int time_to_cut { get; set; }
        }*/

        public List<AppointmentUserInfo> GetAppointmentUserInfoList()
        {
            List<AppointmentUserInfo> appointmentUserInfoList = (from appointment in _db.Appointments
                                                                 join user in _db.Users
                                                                 on appointment.emp_id_chosen equals user.user_id

                                                                 join chosenUser in _db.Users
                                                                 on appointment.emp_id_chosen equals chosenUser.user_id
                                                                 join bookedUser in _db.Users
                                                                 on appointment.user_id_book equals bookedUser.user_id into bookedUsers
                                                                 from bookedUser in bookedUsers.DefaultIfEmpty()
                                                                 join service in _db.Services
                                                                 on appointment.ser_id equals service.ser_id
                                                                 select new AppointmentUserInfo
                                                                 {
                                                                     app_id = appointment.app_id,
                                                                     Date = appointment.date,
                                                                     BookingTime = appointment.booking_time,
                                                                     FinishTime = appointment.finish_time,
                                                                     Status = appointment.status,
                                                                     EmployeeName = user.name,
                                                                     BookedUserName = bookedUser != null ? bookedUser.name : "Not booked",
                                                                     EmployeeIDChosen =appointment.emp_id_chosen,
                                                                     UserIDBooked = (int)appointment.user_id_book,
                                                                     ServiceName = service.name
                                                                 }).ToList();

            return appointmentUserInfoList;
        }
 

        public bool IsAppointmentValid(List<Appointment> list_appointments, TimeOnly booking_time,TimeOnly finish_time)
        {
           /* foreach (var existingAppointment in list_appointments)
            {
                if (booking_time == TimeOnly.Parse(existingAppointment.booking_time))
                {
                    return false;
                }
            }*/

            foreach (var existingAppointment in list_appointments)
            {
                if (booking_time >= TimeOnly.Parse(existingAppointment.booking_time) && booking_time <TimeOnly.Parse(existingAppointment.finish_time) || finish_time > TimeOnly.Parse(existingAppointment.booking_time) && finish_time < TimeOnly.Parse(existingAppointment.finish_time) || finish_time== TimeOnly.Parse(existingAppointment.finish_time))
                {
                    return false;
                }
            }

            return true;
        }

    

        public List<string> GetListOfTimeBooked(DateTime time,int id_imployee)
        {
            List<string> list_time_booked = new List<string>();
           /*if(time.Date == DateTime.Now.Date)
            {

				DateTime now = DateTime.Now;

				DateTime startTime = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);

				list_time_booked.Add(startTime.ToString("HH:mm"));

				while (startTime < now)
				{

					startTime = startTime.AddMinutes(30);

					list_time_booked.Add(startTime.ToString("HH:mm"));
				}
			}*/
            if (time.Date>=DateTime.Now.Date)
            {

                
                    List<Appointment> list = _db.Appointments.Where(a => a.date == time && a.status == "Booking successfully" && a.emp_id_chosen == id_imployee).ToList();

                    foreach (var appointment in list)
                    {
                        TimeOnly.TryParse(appointment.booking_time, out TimeOnly booking_time);
                        TimeOnly.TryParse(appointment.finish_time, out TimeOnly finish_time);
                        
                      
                        list_time_booked.Add(appointment.booking_time.ToString());
                        while (booking_time < finish_time.AddMinutes(-30))
                        {
                            booking_time = booking_time.AddMinutes(30);
                            list_time_booked.Add(booking_time.ToString("HH:mm"));
                        }
                    }
            }
            else
            {
                list_time_booked= new List<string> { "08:00", "08:30", "09:00", "09:30", "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "16:00", "16:30", "17:00" };
            }


            return list_time_booked;
        }

        public int GetEmployeeIfRandom(DateTime date)
        {
            Dictionary<int, int> ListIdUserWithHour = new Dictionary<int, int>();

                int empIdChosen = 0;
                List<User> userList = GetEmployee();
                foreach (User emp in userList)
                {
                    List<Appointment> appointments = _db.Appointments.Where(e => e.emp_id_chosen == emp.user_id && e.date == date && e.status == "Booking successfully").ToList();
                    if (appointments.Count == 0)
                    {
                         empIdChosen= emp.user_id;
                        return empIdChosen;
                    }
                    else
                    {
                        int totalHours = 0;
                        foreach (Appointment appointment in appointments)
                        {
                            Service service = _db.Services.Where(a => a.ser_id == appointment.ser_id).FirstOrDefault();
                            totalHours = totalHours + service.time_to_cut;

                        }
                        ListIdUserWithHour.Add(emp.user_id, totalHours);
                    }
                }
                if (ListIdUserWithHour.Count > 1)
                {
                    int keyIdEmployeeWithMinHour = ListIdUserWithHour.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
                    empIdChosen = keyIdEmployeeWithMinHour;
                    return empIdChosen;
                }

            
            return 0;

        }
        public void CanceledIfLate()
        {
            List<Appointment> appointments = _db.Appointments.ToList();
            if(appointments.Count > 0)
            {
                foreach (Appointment appointment in appointments)
                {

                    DateTime now = DateTime.Now;
                    string hour = now.Hour.ToString("hh:ss");
                    TimeSpan.TryParse(hour, out TimeSpan hourCurrent);
                    List<Appointment> list_app = _db.Appointments.Where(a => a.user_id_book == appointment.user_id_book).ToList();
                    if (list_app.Count != 0)
                    {
                        foreach (var existingAppointment in list_app)
                        {
                            TimeSpan.TryParse(existingAppointment.booking_time, out TimeSpan hourExisting);
                            TimeSpan different = hourCurrent - hourExisting;
                            if (different.TotalMinutes == 2)
                            {
                                existingAppointment.status = "Canceled";
                                _db.Appointments.Update(existingAppointment);
                                _db.SaveChanges();
                            }
                        }

                    }


               }
            }
       
        }




      
    }
}
