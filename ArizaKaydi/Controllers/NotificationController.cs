using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ArizaKaydi.Controllers
{
	[Authorize(Policy = "BasicModeratorViewPermission")]
	public class NotificationController : Controller
	{
		context _context;
		MachineNotificationsManager machineNotificationsManager;

		public NotificationController(context _context)
		{
			this._context = _context;
			this.machineNotificationsManager = new MachineNotificationsManager(new EFMachineNotificationsDal(_context));
		}

		public IActionResult Index()
		{
			var values = machineNotificationsManager.TGetList();
			return View(values);
		}
		public IActionResult RemoveNotification(int id)
		{
			var value = machineNotificationsManager.TGetById(id);
			machineNotificationsManager.TRemove(value);
			return RedirectToAction("Index");
		}
		[HttpGet]
		public IActionResult AddNotification()
		{
			var machines = _context.Machines.Select(m => new SelectListItem
			{
				Value = m.id.ToString(),
				Text = m.name.ToString()
			}).ToList();
			ViewBag.MachineList = machines;

			return View();
			
		}
		[HttpPost]
		public IActionResult AddNotification(machineNotifications machineNotifications)
		{
			if (machineNotifications.machineId == null)
			{
				ModelState.AddModelError("machineId", "Lütfen bir makine seçiniz.");

				// ViewBag'leri tekrar doldurmalısın yoksa dropdown boş kalır
				ViewBag.MachineList = _context.Machines.Select(m => new SelectListItem
				{
					Value = m.id.ToString(),
					Text = m.name
				}).ToList();

				return View(machineNotifications); // Aynı sayfaya geri dön
			}

			machineNotificationsManager.TAdd(machineNotifications);
			return RedirectToAction("Index");
		}
		[HttpGet]
		public IActionResult EditNotification(int id)
		{
			var machines = _context.Machines.Select(m => new SelectListItem
			{
				Value = m.id.ToString(),
				Text = m.name.ToString()
			}).ToList();
			ViewBag.MachineList = machines;
			var values = machineNotificationsManager.TGetById(id);
			return View(values);

		}
		[HttpPost]
		public IActionResult EditNotification(machineNotifications machineNotifications)
		{
			machineNotificationsManager.TUpdate(machineNotifications);
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
	}
}
