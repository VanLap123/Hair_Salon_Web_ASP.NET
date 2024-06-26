﻿using Hair_Salon_Web_ASP.NET.Models;
using Hair_Salon_Web_ASP.NET.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Hair_Salon_Web_ASP.NET.Controllers
{
    public class ChartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RepositoryHairSalon _repo;

        public ChartController(ApplicationDbContext db)
        {
            this._db = db;
            this._repo = new RepositoryHairSalon(db);
        }
        public IActionResult Index(DateTime startTime, DateTime endTime, DateTime startTime1, DateTime endTime1)
        {
            ViewBag.role = HttpContext.Session.GetString("role");
            ViewBag.phone_number = HttpContext.Session.GetString("phone_number");
            if ( startTime.Date != DateTime.MinValue && endTime.Date != DateTime.MinValue) 
            {
                Dictionary<string, float> listPriceEmployee = _repo.GetEmployeeWithNumberAppointment(startTime, endTime);
                List<string> listKey = new List<string>();
                List<float> listValue = new List<float>();
                foreach (var item in listPriceEmployee)
                {
                    listKey.Add(item.Key);
                    listValue.Add(item.Value);
                }
                ViewBag.ListKey = listKey;
                ViewBag.ListValue = listValue;
                float total = listValue.Sum();
                ViewBag.Total = total;
            }

            //////////////////////////
            if (startTime1 != DateTime.MinValue || endTime1 != DateTime.MinValue) 
            {
                List<string> listKeyName = new List<string>();
                List<int> listValueNumber = new List<int>();
                Dictionary<string, int> listNameAndNumber = _repo.GetNumberAppointMentByEachEmployee(startTime1, endTime1);
                foreach (var item in listNameAndNumber)
                {
                    listKeyName.Add(item.Key);
                    listValueNumber.Add(item.Value);
                }
                ViewBag.ListKeyName = listKeyName;
                ViewBag.ListValueNumber = listValueNumber;
            }
        
            return View();
        }
       
    }
}
