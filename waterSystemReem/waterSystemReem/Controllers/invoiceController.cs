using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using waterSystemReem.DATA;
using waterSystemReem.Models;
namespace waterSystemReem.Controllers
{
    public class invoiceController : Controller
    {
        private readonly WaterSystemOrgContext _db;

        public invoiceController(WaterSystemOrgContext db)
        {
            _db = db;
        }

        // عرض كل فواتير العداد
        public IActionResult displayAllInvoices(int id) 
        {
            var invoicesList = _db.Invoices
                              .FromSqlRaw("EXEC GetInvoicesByCounter @p0", id)
                              .ToList();

            // تحميل بيانات العداد لكل فاتورة (إذا كانت القائمة غير فارغة)
            foreach (var invoice in invoicesList)
            {
                _db.Entry(invoice).Reference(i => i.Counter).Load();
            }

            // تخزين الـ CounterId في ViewBag
            ViewBag.CounterId = id;

            return View("displayAllInvoices", invoicesList);
        }


        // GET: عرض تفاصيل الفاتورة
        public async Task<IActionResult> print(int id)
        {
            var invoiceDetails = await _db.InvoiceCounterViews
                .FromSqlRaw("EXEC GetInvicDetails @p0", new object[] { id })
                .ToListAsync();

            if (!invoiceDetails.Any())
            {
                return NotFound("the invoice not found!");
            }

            return View(invoiceDetails);
        }

        // POST: عملية الدفع
        [HttpPost]
        public async Task<IActionResult> print(int invoiceId, decimal invoiceBalance, int counterId)
        {
            try
            {
                // استدعاء الإجراء المخزن الذي يقوم بكل العملية (التحقق من الدفع والتحديث)
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"EXEC PayInvoice {invoiceId}, {invoiceBalance}, {counterId}");

                return Json(new { success = true, message = "the payment is done success" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        //// GET: عرض صفحة إضافة فاتورة جديدة
        //[HttpGet]
        //public IActionResult Create(int counterId)  // id هنا هو CounterId
        //{
        //    var model = new InvoiceCreateViewModel
        //    {
        //        CounterId = counterId
        //    };
        //    return View(model);
        //}


        //[HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var result = await _db.Set<GetCounterDatesFirstStage>()
                      .FromSqlRaw($"SELECT * FROM dbo.GetCounterDates({id})")
                      .AsNoTracking()
                      .ToListAsync();


            if (result == null)
            {
                return NotFound(); // ✅ التعامل مع حالة عدم وجود بيانات
            }
            var model = new InvoiceCreateViewModel
            {
                CounterId = id,
                FromDate = result.First().FromDate,
                ToDate = result.First().ToDate,
            };

            return View(model);
        }








        // POST: عند تقديم النموذج، يتم استدعاء الإجراء المخزن sp_AddInvoice
        [HttpPost]
        public async Task<IActionResult> Create(InvoiceCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // استدعاء الإجراء المخزن باستخدام ExecuteSqlInterpolatedAsync
                    await _db.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC sp_AddInvoice {model.CounterId}, {model.PrevReading}, {model.CurrReading}");

                    // بعد الإضافة، يمكنك إعادة التوجيه لعرض جميع الفواتير الخاصة بهذا العداد
                    return RedirectToAction("displayAllInvoices", new { id = model.CounterId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }


        // أكشن للتحقق من عدد الأشهر المنقضية باستخدام الدالة fn_MonthsPassed
        [HttpGet]
        public async Task<IActionResult> CheckMonthsPassed(int counterId)
        {
            int months = 0;
            using (var conn = _db.Database.GetDbConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT dbo.fn_MonthsPassed(@counterId)";
                    var param = cmd.CreateParameter();
                    param.ParameterName = "@counterId";
                    param.Value = counterId;
                    cmd.Parameters.Add(param);
                    var result = await cmd.ExecuteScalarAsync();
                    months = Convert.ToInt32(result);
                }
            }
            return Json(new { monthsPassed = months });
        }
    }
}
