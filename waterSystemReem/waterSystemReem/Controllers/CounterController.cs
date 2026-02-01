using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using waterSystemReem.DATA;
using waterSystemReem.Models;
using System.Collections.Concurrent;

namespace waterSystemReem.Controllers
{
    public class CounterController : Controller
    {
        private readonly WaterSystemOrgContext _context;
        public CounterController(WaterSystemOrgContext context)
        {
            _context = context;
        }
        public IActionResult Show()
        {
            var counters = _context.Set<CounterListViewModel>()
                .FromSqlRaw("EXEC GetAllCounters")
                .ToList();

            return View("Show", counters);
        }

        [HttpGet]
        public async Task<IActionResult> Show(string searchString)
        {
            ViewBag.SearchString = searchString;

            var query = _context.Counters
                                .Include(c => c.CounterType) // Eager load related CounterType
                                .AsQueryable(); // Make it IQueryable

            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumericId = int.TryParse(searchString, out int searchId);
                bool isNumericNumber = long.TryParse(searchString, out long searchNum);

                query = query.Where(c =>
                    (isNumericId && c.CounterId == searchId) ||
                    (isNumericNumber && c.CounterNumber == searchNum) ||
                    (c.CounterLocation != null && c.CounterLocation.Contains(searchString)) ||
                    (c.CounterType != null && c.CounterType.CounterTypeName != null && c.CounterType.CounterTypeName.Contains(searchString))
                );
            }


            var countersViewModel = await query.Select(c => new CounterListViewModel
            {
                CounterId = c.CounterId,
                CounterNumber = c.CounterNumber,
                CounterLocation = c.CounterLocation,
                DateLastReading = c.DateLastReading,
                PreviseBalance = c.PreviseBalance,
                // SubscriberId = c.SubscriberId,    // Include if CounterListViewModel needs it
                // CounterTypeId = c.CounterTypeId,  // Include if CounterListViewModel needs it
                CounterTypeName = c.CounterType != null ? c.CounterType.CounterTypeName : "N/A" // Handle case where CounterType might be null
            })
            .ToListAsync();

            return View(countersViewModel);
        }


        // Overloaded action for search; returns CounterListViewModel as well.
        //[HttpGet]
        //public IActionResult Show(string searchString)
        //{
        //    List<CounterListViewModel> counters;
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        counters = _context.Set<CounterListViewModel>()
        //            .FromSqlRaw("EXEC SearchCounter @SearchString",
        //                        new SqlParameter("@SearchString", searchString))
        //            .ToList();
        //    }
        //    else
        //    {
        //        counters = _context.Set<CounterListViewModel>()
        //            .FromSqlRaw("EXEC GetAllCounters")
        //            .ToList();
        //    }

        //    return View("Show", counters);
        //}
        public IActionResult Details(int id)
        {
            var counterDetails = _context.CounterDetailsViews
                .FromSqlRaw("EXEC GetCounterDetails @CounterId", new SqlParameter("@CounterId", id))
                .ToList();

            if (counterDetails == null || !counterDetails.Any())
            {
                return NotFound();
            }

            return View(counterDetails);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddCounterViewModel model)
        {
            Console.WriteLine($"Counter Number: {model.CounterNumber}");
            Console.WriteLine($"Counter Location: {model.CounterLocation}");
            Console.WriteLine($"Previse Balance: {model.PreviseBalance}");
            Console.WriteLine($"Subscriber Id: {model.SubscriberId}");
            Console.WriteLine($"Counter Type Id: {model.CounterTypeId}");

            if (ModelState.IsValid)
            {
                try
                {
                    var counterNumberParam = new SqlParameter("@counter_number", model.CounterNumber);
                    var counterLocationParam = new SqlParameter("@counter_location", model.CounterLocation);
                    var previseBalanceParam = new SqlParameter("@previse_balance", model.PreviseBalance);
                    var subscriberIdParam = new SqlParameter("@subscriber_id", model.SubscriberId);
                    var counterTypeIdParam = new SqlParameter("@counter_type_id", model.CounterTypeId);

                    // التحقق من القيم المدخلة
                    if (model.CounterTypeId != 1 && model.CounterTypeId != 2)
                    {
                        ModelState.AddModelError("CounterTypeId", "Invalid Counter Type. Please enter 1 for Commercial or 2 for Home.");
                        return View(model);
                    }

                    // تنفيذ الاستعلام
                    _context.Database.ExecuteSqlRaw(
                        "EXEC AddCounter @counter_number, @counter_location, @previse_balance, @subscriber_id, @counter_type_id",
                        counterNumberParam,
                        counterLocationParam,
                        previseBalanceParam,
                        subscriberIdParam,
                        counterTypeIdParam);

                    return RedirectToAction("Show");
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine("SQL Error: " + sqlEx.Message);
                    ModelState.AddModelError("", "Database error: " + sqlEx.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("General Error: " + ex.Message);
                    ModelState.AddModelError("", "An unexpected error occurred.");
                }
            }

            return View(model);
        }


    }
}

//scaffold - dbcontext "server=DESKTOP-2B10KMP\G2;database=WaterSystemOrg;trusted_connection=true"Microsoft.EntityFrameworkCore.sqlserver - outputdir Models - contextdir Data - force
