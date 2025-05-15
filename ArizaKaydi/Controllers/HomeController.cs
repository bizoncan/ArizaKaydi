using ArizaKaydi.Models;
using DataAccessLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ArizaKaydi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly context _context;

		public HomeController(ILogger<HomeController> logger, context _context)
        {
            _logger = logger;
			this._context = _context;
		}

		

		public IActionResult Index()
        {
            ViewBag.WorkOrderCount = _context.WorkOrders.Where(e=> e.isClosed != true).Count();
            ViewBag.UserCount = _context.mobileUsers.Count();
            ViewBag.ReportCount = _context.WorkOrders.Count();
            var worksF = _context.WorkOrders.Where(e=>e.isClosed==true).ToList();
            double totalTime = 0.0;
			foreach (var item in worksF)
			{

				totalTime += (item.workOrderEndDate - item.workOrderStartDate).Value.TotalHours;

			}

            ViewBag.MeanFinish = totalTime / worksF.Count();
			/*var totalPairTime = 0.0;
            foreach(var item in _context.Errors.ToList())
            {
				if (item.errorEndDate != null)
				{
					totalPairTime += (item.errorEndDate - item.errorDate).TotalHours;
				}
			}
            ViewBag.MTTR = totalPairTime / _context.Errors.Count();*/
			var m = _context.WorkOrders.Include(x=>x.machine).Include(x=>x.machinePart).Include(x=> x.userI).ToList();
			return View(m);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
