using ArizaKaydi.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ArizaKaydi.Controllers
{
	public class WorkController : Controller
	{
		WorkManager workManager;
		WorkOrderManager workOrderManager;
		ImageCollectionManager imageCollectionManager;
		context _context;

		public WorkController(context _context)
		{
			this._context = _context;
			this.workManager = new WorkManager(new EFWorkDal(_context));
			this.workOrderManager = new WorkOrderManager(new EFWorkOrderDal(_context));
			imageCollectionManager = new ImageCollectionManager(new EFImageCollectionDal(_context));
		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Index()
		{
			var values = workOrderManager.TGetList();
			return View(values);
		}
		public IActionResult RemoveWorkOrder(int id)
		{
			var workOrder = workOrderManager.TGetById(id);
			if (workOrder != null)
			{
				workOrderManager.TRemove(workOrder);
				return RedirectToAction("Index");
			}
			return NotFound();
		}
		public IActionResult DeleteWork(int id)
		{
			var workk = workManager.TGetById(id);
			if (workk != null)
			{
				workManager.TRemove(workk);
				return RedirectToAction("Index");
			}
			return NotFound();
		}
		[HttpGet]
		public IActionResult AddWorkOrder()
		{
			var machines = _context.Machines.Select(m => new SelectListItem
			{
				Value = m.id.ToString(),
				Text = m.name.ToString()
			}).ToList();
			ViewBag.MachineList = machines;
			var users = _context.Users.Select(m => new SelectListItem
			{
				Value = m.Id.ToString(),
				Text = m.UserName.ToString()
			}).ToList();
			ViewBag.UserList = users;
			return View();
		}
		[HttpPost]
		public IActionResult AddWorkOrder(workOrder p)
		{
			p.workOrderStartDate = new DateTime(
				DateTime.Now.Year,
				DateTime.Now.Month,
				DateTime.Now.Day,
				DateTime.Now.Hour,
				DateTime.Now.Minute,
				DateTime.Now.Second
			); 
			if (p.userId != null)
			{
				p.isOpened = true;
			}
			workOrderManager.TAdd(p);
			return RedirectToAction("Index");
		}
		[HttpGet]
		public IActionResult GetMachineParts(int machineId)
		{
			var parts = _context.MachineParts
				.Where(p => p.machineId == machineId)
				.Select(p => new SelectListItem
				{
					Value = p.Id.ToString(),
					Text = p.name
				}).ToList();

			return Json(parts); // ← Tarayıcıya JSON veri gönder
		}
		public IActionResult EditWorkOrder(int id)
		{

			var machines = _context.Machines.Select(m => new SelectListItem
			{
				Value = m.id.ToString(),
				Text = m.name.ToString()
			}).ToList();
			ViewBag.MachineList = machines; // Use the same name as in AddError for consistency
			var users = _context.Users.Select(m => new SelectListItem
			{
				Value = m.Id.ToString(),
				Text = m.UserName.ToString()
			}).ToList();
			ViewBag.UserList = users;

			var value = workOrderManager.TGetById(id);
			return View(value);
		}
		[HttpPost]
		public IActionResult EditWorkOrder(workOrder p)
		{
			var existing = workOrderManager.TGetById(p.id); // Bu DbContext tarafından takip ediliyor

			if (existing == null)
				return NotFound();

			// Sadece değişen alanları elle set et
			existing.machineId = p.machineId;
			existing.machinePartId = p.machinePartId;
			existing.title = p.title;
			existing.desc = p.desc;
			existing.isOpened = p.isOpened;
			existing.isClosed = p.isClosed;
			if (p.userId != null)
			{
				existing.userId = p.userId;
				existing.isOpened = true;
				existing.workOrderTempStartDate = new DateTime(
				DateTime.Now.Year,
				DateTime.Now.Month,
				DateTime.Now.Day,
				DateTime.Now.Hour,
				DateTime.Now.Minute,
				DateTime.Now.Second
			);
			}
			else
			{
				existing.userId = p.userId;
				existing.isOpened = false;
			}
			if (!p.isClosed)
			{
				existing.workOrderEndDate = null;
			}
			
			workOrderManager.TUpdate(existing);
			return RedirectToAction("Index");
		}
		
		public IActionResult WorkReports(int id)
		{
			var workReports = _context.Works.Where(x => x.workOrderId == id).ToList();
			if (workReports.Count != 0)
			{
				var works = workManager.TGetWorkByWorkOrderList(id);
				return View(works);
			}
			else
			{
				TempData["SwalMessage"] = "Güncel iş raporu daha eklenmedi.";
				TempData["SwalType"] = "warning";
				return RedirectToAction("Index");
			}
		}
		public IActionResult SeeWorkReport(int id)
		{

			var works = workManager.TGetWorkWithWorkOrder(id);
			var images = imageCollectionManager.TGetImageCollectionByWorkId(works.id);
			List<string> imageList = new List<string>();
			if (images.Count > 0)
			{
				string imreBase64Data;
				foreach (var item in images)
				{
					imreBase64Data = Convert.ToBase64String(item.imageDataByte);
					imageList.Add(string.Format("data:image/jpeg;base64,{0}", imreBase64Data));
				}
			}
			var er_im = new WorkDetailModel
			{
				workInfo = works,
				collection = imageList
			};
			return View(er_im);

		}
		public IActionResult WorkOrderDetails(int id)
		{
			var value = _context.WorkOrders.Include(x=> x.machine).Include(x => x.userI).Include(x => x.machinePart).FirstOrDefault(x => x.id == id);
			return View(workOrderManager.TGetById(id));
		}
	}
}
