using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using waterSystemReem.DATA;
using waterSystemReem.Models;
using System.Collections.Concurrent;

namespace waterSystemReem.Controllers
{
    public class SubscriberController : Controller
    {
        private readonly WaterSystemOrgContext _context;
        public SubscriberController(WaterSystemOrgContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Subscriber> subscribers;
            subscribers = _context.Subscribers
                    .FromSqlRaw("EXEC ShowSubscribers ")
                    .ToList();
            return View(subscribers);
        }
        public IActionResult Create()
        {
            return View();
        }

        // معالجة طلب إضافة عميل جديد باستخدام Stored Procedure
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Subscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                var identeParam = new SqlParameter("@identification", subscriber.IdentificationNumber);
                var nameParam = new SqlParameter("@sub_name", subscriber.SubscriberName);
                var phoneParam = new SqlParameter("@sub_phone", subscriber.SubscriberPhone);

                _context.Database.ExecuteSqlRaw("EXEC AddSubscriber @identification, @sub_name, @sub_phone", identeParam, nameParam, phoneParam);
                return RedirectToAction(nameof(Index)); // إعادة التوجيه إلى قائمة العملاء
            }
            return View(subscriber);
        }
        [HttpGet]
        public IActionResult Index(string searchString)
        {
            List<Subscriber> subscriber;

            if (!string.IsNullOrEmpty(searchString))
            {
                subscriber = _context.Subscribers
                    .FromSqlRaw("EXEC SearchSubscriber @SearchString", new SqlParameter("@SearchString", searchString))
                    .ToList();
            }
            else
            {
                subscriber = _context.Subscribers.ToList();
            }

            return View(subscriber);
        }
        public IActionResult ShowDetiles(int id)
        {
            var subscriberDetails = _context.SubscriberCountersViews.FromSqlRaw
                ("EXEC GetSubscriberDetails @SubscriberId", new SqlParameter
                ("@SubscriberId", id))
           .ToList();
            ViewBag.subscriber = _context.Subscribers.FirstOrDefault(x => x.SubscriberId == id);

            return View(subscriberDetails);
        }
        public IActionResult Delete(int id)
        {
            var subscriberDetails = _context.SubscriberCountersViews.FromSqlRaw
                ("EXEC GetSubscriberDetails @SubscriberId", new SqlParameter
                ("@SubscriberId", id))
           .ToList();
            ViewBag.subscriber = _context.Subscribers.FirstOrDefault(x => x.SubscriberId == id);
            return View(subscriberDetails);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deleted(int id)
        {
            var masseg = _context.Database.ExecuteSqlRaw("EXEC DeleteSubscriber @SubscriberId", new SqlParameter
                ("@SubscriberId", id));
            return RedirectToAction(nameof(Index));
        }

    }
}
