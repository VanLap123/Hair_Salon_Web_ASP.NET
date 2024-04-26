﻿using Hair_Salon_Web_ASP.NET.Models;
using Hair_Salon_Web_ASP.NET.Repository;
using Hair_Salon_Web_ASP.NET.UserFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Globalization;


namespace Hair_Salon_Web_ASP.NET.Controllers
{
   
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RepositoryHairSalon _repo;
        
        public AppointmentController(ApplicationDbContext db)
        {
            this._db = db;
            this._repo=new RepositoryHairSalon(db);
        }
        [UserRole(UserRole.Admin)]
        public async Task<IActionResult> Index(string phoneNumber,string date)
        {

            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            List<AppointmentUserInfo> appointments =_repo.GetAppointmentUserInfoList();
            if (!string.IsNullOrEmpty(phoneNumber))
            {   User user =_db.Users.Where(u=>u.phone_number == phoneNumber).SingleOrDefault();
                
                if (user!=null)
                {
                    List<Appointment> appointments1 = _db.Appointments.Where(s => s.user_id_book == user.user_id).ToList();
                    if (appointments1.Count() != 0)
                    {
                        return View(appointments1);
                    }
                    else
                    {
                        ViewBag.SearchMessage = "The Appointment is not found!";
                    }
                }
                else
                {
                    ViewBag.SearchMessage = "The Appointment is not found!";
                }
                   
            }
            if(date!=null)
            {
                DateTime dateChosen =DateTime.Parse(date);
                List<Appointment> appointments2 = _db.Appointments.Where(s => s.date == dateChosen).ToList();
                if(appointments2.Count() != 0)
                {
                    return View(appointments2);
                }
                else
                {
                    ViewBag.SearchMessageDate = "The Appointment is not found!";
                }
            }

            return View(appointments);
        }

        public IActionResult GetBookedTimes(DateTime selectedDate, int empIdChosen)
        {
           Dictionary<int,int> ListIdUserWithHour = new Dictionary<int,int>();

            List<string> bookedTimes = new List<string>();
            if (empIdChosen == 1)
            {
                List<User> userList = _repo.GetEmployee();
                foreach(User user in userList)
                {
                   List<Appointment> appointments = _db.Appointments.Where(e => e.emp_id_chosen == user.user_id && e.date == selectedDate && e.status == "Booking successfully").ToList();
                    if(appointments.Count == 0)
                    {
                        empIdChosen=user.user_id;
                        bookedTimes = _repo.GetListOfTimeBooked(selectedDate, empIdChosen);
                        return Ok(bookedTimes);
                    }
                    else
                    {
                        int totalHours = 0;
                        foreach (Appointment appointment in appointments)
                        {
                            Service service = _db.Services.Where(a => a.ser_id == appointment.ser_id).FirstOrDefault();
                            totalHours = totalHours + service.time_to_cut;

                        }
                        ListIdUserWithHour.Add(user.user_id, totalHours);
                    }   
                }
                if (ListIdUserWithHour.Count >1)
                {
                    int keyIdEmployeeWithMinHour = ListIdUserWithHour.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
                    empIdChosen = keyIdEmployeeWithMinHour;
                    bookedTimes = _repo.GetListOfTimeBooked(selectedDate, empIdChosen);
                    return Ok(bookedTimes);
                }  

            }

            bookedTimes = _repo.GetListOfTimeBooked(selectedDate, empIdChosen);
            return Ok(bookedTimes);
        }
        
        public IActionResult Create(int ser_id)
        {
            string role = HttpContext.Session.GetString("role");
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            Service service = new Service();
            Appointment appointment = new Appointment();
            if (ser_id != null)
            {
                //when customer choose service in services page
                service = _db.Services.Where(s=>s.ser_id == ser_id).FirstOrDefault();
                
                appointment.ser_id = ser_id;
                appointment.date= DateTime.Now.Date;
            }
           
            if (role ==null)   
            {
                TempData["LoginMessage"] = "You need to log in to book a haircut or create a new account.";
                return RedirectToAction("Index", "Login");
            }
                ViewData["ser_id"] = new SelectList(_db.Services, "ser_id", "name");

                List<User> emp_list = _repo.GetEmployee();
                ViewData["emp_list"] = emp_list;
                return View(appointment);
        }
        
        [HttpPost]
        public IActionResult Create(Appointment appointment)
        {
            List<User> emp_list = _repo.GetEmployee();
            ViewData["emp_list"] = emp_list;
			ViewData["ser_id"] = new SelectList(_db.Services, "ser_id", "ser_id", appointment.ser_id);
            ViewData["emp_id_chosen"] = new SelectList(_repo.GetEmployee(), "user_id", "emp_id_chosen", appointment.emp_id_chosen);

            if (appointment.emp_id_chosen == 1)
            {
                    int empId= _repo.GetEmployeeIfRandom(appointment.date);
                        appointment.emp_id_chosen = empId;
            }

            if (ModelState.IsValid)
            {
                Service found_service = _db.Services.SingleOrDefault(s => s.ser_id == appointment.ser_id);
                appointment.user_id_book = HttpContext.Session.GetInt32("user_id");
                if (TimeOnly.TryParse(appointment.booking_time, out TimeOnly newTimeOnly))
                {                   
                    TimeOnly finish_time = newTimeOnly.AddMinutes(found_service.time_to_cut);
                    appointment.finish_time = finish_time.ToString();
                }

                if (appointment.date.Date >= DateTime.Now.Date)
                {
                    List<Appointment> appointments = _repo.GetAppointmentsByEmp_IdAndDate(appointment.date, appointment.emp_id_chosen);
                    if (_repo.IsAppointmentValid(appointments, newTimeOnly, newTimeOnly.AddMinutes(found_service.time_to_cut)) == true)
                    {
                        appointment.status = "Booking successfully";
                        _db.Add(appointment);
                        _db.SaveChanges();
                        if(HttpContext.Session.GetString("role")=="Admin")
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            // Register Successfully here, I need notification to customer
                            TempData["SuccessMessage"] = "Booking successfully";
                            return RedirectToAction("Index","Home"); 
                        }
                        
                    }
                    else
                    {
                       
                        ModelState.AddModelError(string.Empty, "The service you choose not enough time, please choose another:");
                        return View("Create");
                    }
                }
                else
                {

                    ModelState.AddModelError("date", "You can not choose the date in the past");
                    return View("Create");
                }
                      
            }

            return View(appointment);
        }


        [UserRole(UserRole.Employee)]
        public IActionResult ShowTaskEmployee(string date_show_task) 
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            int user_id = HttpContext.Session.GetInt32("user_id") ?? 0;
            DateTime now = DateTime.Now;
            DateTime currentDate = new DateTime(now.Year, now.Month, now.Day, 00, 00, 00);
            List<AppointmentUserInfo> appointments = _repo.GetAppointmentUserInfoList().Where(e => e.EmployeeIDChosen == user_id && e.Date==currentDate && e.Status == "Booking successfully").ToList();
             //Filter herel
            if (date_show_task != null)
            {
                DateTime dateChosen = DateTime.Parse(date_show_task);
                List<AppointmentUserInfo> appointments2 = _repo.GetAppointmentUserInfoList().Where(e => e.EmployeeIDChosen == user_id && e.Date ==dateChosen && e.Status == "Booking successfully").ToList();
                if (appointments2.Count() != 0)
                {
                    return View(appointments2);
                }
                else
                {
                    ViewBag.SearchMessageDate = "The Appointment is not found!";
                }
            }
            return View(appointments);
        }
        [UserRole(UserRole.Client)]
        public IActionResult ShowAppointmentCurrent()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            int user_id = HttpContext.Session.GetInt32("user_id") ?? 0;
            List<AppointmentUserInfo> list_app=_repo.GetAppointmentsByID(user_id);

            
            return View(list_app);
        }
        [UserRole(UserRole.Client)]
        public IActionResult ShowBookingHistory()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            int user_id = HttpContext.Session.GetInt32("user_id") ?? 0;
            List<AppointmentUserInfo> list_app = _repo.GetAppointmentsHistory(user_id);
            return View(list_app);
        }
        [UserRole(UserRole.Client)]
        public IActionResult ShowCanceledHistory()
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            int user_id = HttpContext.Session.GetInt32("user_id") ?? 0;
            List<AppointmentUserInfo> list_app = _repo.GetAppointmentsCanceled(user_id);
            return View(list_app);
        }
        [UserRole(UserRole.Client)]
        public IActionResult CanceledByClient(int app_id)
        {
            _repo.UpdateAppointmentStatus(app_id, "Canceled");
            return View("ShowAppointmentCurrent");
        }
        [UserRole(UserRole.Admin, UserRole.Employee)]
        [HttpGet]
        public IActionResult Accept(int app_id)
        {
            _repo.UpdateAppointmentStatus(app_id, "Booking successfully");
            return RedirectToAction("Index");
        }
        [UserRole(UserRole.Admin, UserRole.Employee)]
        [HttpGet]
        public IActionResult Refuse(int app_id)
        {
            _repo.UpdateAppointmentStatus(app_id, "Canceled");
            if (HttpContext.Session.GetString("role") == "Employee")
            {
                return RedirectToAction("ShowTaskEmployee");
            }
            return RedirectToAction("Index");
        }

        [UserRole(UserRole.Admin, UserRole.Employee)]
        [HttpGet]
        public IActionResult Done(int app_id)
        {
            _repo.UpdateAppointmentStatus(app_id, "Finished");
            return RedirectToAction("Index");
        }
        [UserRole(UserRole.Admin, UserRole.Employee)]
        public IActionResult Edit(int app_id)
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            ViewData["ser_id"] = new SelectList(_db.Services, "ser_id", "name");
            ViewData["emp_list"] = _repo.GetEmployee();

            if (app_id == null || _db.Appointments == null)
            {
                return NotFound();
            }

            Appointment appointment = _db.Appointments.Find(app_id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }
        [UserRole(UserRole.Admin, UserRole.Employee)]
        [HttpPost]
        public IActionResult Edit(int app_id, Appointment appointment)
        {
            ViewData["ser_id"] = new SelectList(_db.Services, "ser_id", "name");
            ViewData["emp_list"] = _repo.GetEmployee();

            if (app_id != appointment.app_id)
            {
                return NotFound();
            }
            if (appointment.emp_id_chosen == 1)
            {
                int empId = _repo.GetEmployeeIfRandom(appointment.date);
                appointment.emp_id_chosen = empId;
            }

            if (ModelState.IsValid)
            {
                Service found_service = _db.Services.SingleOrDefault(s => s.ser_id == appointment.ser_id);
                appointment.user_id_book = HttpContext.Session.GetInt32("user_id");

                if (TimeOnly.TryParse(appointment.booking_time, out TimeOnly newTimeOnly))
                {
                    TimeOnly finish_time = newTimeOnly.AddMinutes(found_service.time_to_cut);
                    appointment.finish_time = finish_time.ToString();
                }

                if (appointment.date.Date >= DateTime.Now.Date)
                {
                    List<Appointment> appointments = _repo.GetAppointmentsByEmp_IdAndDate(appointment.date, appointment.emp_id_chosen);
                    if (_repo.IsAppointmentValid(appointments, newTimeOnly, newTimeOnly.AddMinutes(found_service.time_to_cut)) == true)
                    {
                        appointment.status = "Booking successfully";
                        _db.Update(appointment);
                        _db.SaveChanges();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "This time is booked, please choose another:");
                        return View("Edit");
                    }
                }
                else
                {

                    ModelState.AddModelError("date", "You can not choose the date in the past");
                    return View("Edit");
                }

            }
            return View(appointment);
        }

        public IActionResult CreateAppointment(int ser_id)
        {
            string role = HttpContext.Session.GetString("role");
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            Service service = new Service();
            Appointment appointment = new Appointment();
            if (ser_id != null)
            {
                //when customer choose service in services page
                service = _db.Services.Where(s => s.ser_id == ser_id).FirstOrDefault();

                appointment.ser_id = ser_id;
                appointment.date = DateTime.Now.Date;
            }

            if (role == null)
            {
                TempData["LoginMessage"] = "You need to log in to book a haircut or create a new account.";
                return RedirectToAction("Index", "Login");
            }
            ViewData["ser_id"] = new SelectList(_db.Services, "ser_id", "name");

            List<User> emp_list = _repo.GetEmployee();
            ViewData["emp_list"] = emp_list;
            return View(appointment);
        }

        [HttpPost]
        public IActionResult CreateAppointment(Appointment appointment, string phoneNumber)
        {
            List<User> emp_list = _repo.GetEmployee();
            ViewData["emp_list"] = emp_list;
            ViewData["ser_id"] = new SelectList(_db.Services, "ser_id", "ser_id", appointment.ser_id);
            ViewData["emp_id_chosen"] = new SelectList(_repo.GetEmployee(), "user_id", "emp_id_chosen", appointment.emp_id_chosen);

            if (appointment.emp_id_chosen == 1)
            {
                int empId = _repo.GetEmployeeIfRandom(appointment.date);
                appointment.emp_id_chosen = empId;
            }

            if (ModelState.IsValid)
            {
                Service found_service = _db.Services.SingleOrDefault(s => s.ser_id == appointment.ser_id);

                User user = _repo.GetUserByPhoneNumber(phoneNumber);
                appointment.user_id_book = user.user_id;

                if (TimeOnly.TryParse(appointment.booking_time, out TimeOnly newTimeOnly))
                {
                    TimeOnly finish_time = newTimeOnly.AddMinutes(found_service.time_to_cut);
                    appointment.finish_time = finish_time.ToString();
                }

                if (appointment.date.Date >= DateTime.Now.Date)
                {
                    List<Appointment> appointments = _repo.GetAppointmentsByEmp_IdAndDate(appointment.date, appointment.emp_id_chosen);
                    if (_repo.IsAppointmentValid(appointments, newTimeOnly, newTimeOnly.AddMinutes(found_service.time_to_cut)) == true)
                    {
                        appointment.status = "Booking successfully";
                        _db.Add(appointment);
                        _db.SaveChanges();
                        if (HttpContext.Session.GetString("role") == "Admin")
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            // Register Successfully here, I need notification to customer
                            TempData["SuccessMessage"] = "Booking successfully";
                            return RedirectToAction("Index", "Home");
                        }

                    }
                    else
                    {

                        ModelState.AddModelError(string.Empty, "The service you choose not enough time, please choose another:");
                        return View("Create");
                    }
                }
                else
                {

                    ModelState.AddModelError("date", "You can not choose the date in the past");
                    return View("Create");
                }

            }

            return View(appointment);
        }
        [UserRole(UserRole.Admin, UserRole.Employee)]
        public IActionResult Delete(int app_id)
        {
            Appointment obj = _db.Appointments.Find(app_id);
            if (obj != null)
            {
                _db.Appointments.Remove(obj);
                _db.SaveChanges();

            }
            if (HttpContext.Session.GetString("role") == "Employee")
            {
                return RedirectToAction("ShowTaskEmployee");
            }
            return RedirectToAction("Index");
        }
    }
}