using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKaydi.Controllers
{
	public class CalculationController : Controller
	{
		context _context;
		ErrorManager errorManager;
		MtQueriesManager mtQueriesManager;
		public CalculationController(context context)
		{
			_context = context;
			this.errorManager = new ErrorManager(new EFErrorDal(_context));
			mtQueriesManager = new MtQueriesManager(new EFMtQueriesDal(_context));
		}

		public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Index(DateTime StartDate, DateTime StartTime, DateTime EndDate, DateTime EndTime, string calculationType)
		{
			DateTime startDateTime = StartDate.Date + StartTime.TimeOfDay;
			DateTime endDateTime = EndDate.Date + EndTime.TimeOfDay;
			var errors= errorManager.TGetErrorsByDate(startDateTime, endDateTime);
			
			double result = (endDateTime - startDateTime).TotalHours;
			if (calculationType == "MTTR")
			{
				double totalrepairtime = 0;
				double totalerrors = errors.Count();
				if (totalerrors == 0)
				{
					TempData["Result"] = "Bu tarih aralığında hata bulunamadı";
					return RedirectToAction("MTTR");
				}
				foreach (var error in errors)
				{
					totalrepairtime = totalrepairtime + (error.errorEndDate - error.errorDate).TotalHours;
				}
				result = totalrepairtime / totalerrors;
				mtQueries m = new mtQueries();
				m.startDate = startDateTime;
				m.endDate = endDateTime;
				m.queryType = "MTTR";
				m.errorCount = errors.Count();
				m.value = result;
				mtQueriesManager.TAdd(m);
				TempData["Result"] = result.ToString();
				return RedirectToAction("MTTR");
			}
			else if (calculationType == "MTBF")
			{
				double totalerrors = errors.Count();
				if (totalerrors > 0)
				{
					result = result / totalerrors;// bu değer tam doğru değil. makinenin çalışma süresi bilinmeden hesaplanması doğru değil
					mtQueries m = new mtQueries();
					m.startDate = startDateTime;
					m.endDate = endDateTime;
					m.queryType = "MTBF";
					m.errorCount = errors.Count();
					m.value = result;
					mtQueriesManager.TAdd(m);
					TempData["Result"] = result.ToString();
					return RedirectToAction("MTBF");
				}
				else
				{
					TempData["Result"] = "Bu tarih aralığında hata bulunamadı";
					return RedirectToAction("MTBF");
				}

				
			}
			return View();
		}
		public IActionResult MTTR()
		{
			ViewBag.Result = TempData["Result"];
			return View();
		}

		public IActionResult MTBF()
		{
			ViewBag.Result = TempData["Result"];
			return View();
		}
	}
}
