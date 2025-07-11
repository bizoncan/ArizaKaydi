using ArizaKaydi.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;

namespace ArizaKaydi.Controllers
{
	[Authorize(Policy = "BasicModeratorViewPermission")]
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
		public IActionResult Index(DateTime? startDate = null, DateTime? endDate = null, int? machineId = null, int? machinePartId = null, string? sortBy=null, string? sortOrder=null, Boolean? isClosed=null,int? userId=null)
		{
			var values = _context.WorkOrders.Include(x => x.machine).Include(x=> x.userI).ToList();

			ViewBag.Machines = _context.Machines.ToList(); 

			ViewBag.MobileUsers = _context.mobileUsers.ToList();

			List<machinePart> partsForViewBag = new List<machinePart>();

			if (startDate.HasValue)
			{
				values = values.Where(e => e.workOrderStartDate >= startDate.Value).ToList();
				ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
			}

			if (endDate.HasValue)
			{
				var endOfDay = endDate.Value.Date.AddDays(1).AddSeconds(-1);
				values = values.Where(e => e.workOrderStartDate <= endOfDay).ToList();
				ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
			}

			if(userId.HasValue && userId.Value > 0)
			{
				values = values.Where(e => e.userId == userId.Value).ToList();
				ViewBag.SelectedUserId = userId.Value;
			}
			else
			{
				ViewBag.SelectedUserId = null; 
			}

			// Makine Filtresi
			if (machineId.HasValue && machineId.Value > 0)
			{
				values = values.Where(e => e.machineId == machineId.Value).ToList();
				ViewBag.SelectedMachineId = machineId.Value;

				partsForViewBag = _context.MachineParts
									  .Where(mp => mp.machineId == machineId.Value)
									  .ToList();

				if (machinePartId.HasValue && machinePartId.Value > 0)
				{
					if (partsForViewBag.Any(p => p.Id == machinePartId.Value))
					{
						values = values.Where(e => e.machinePartId == machinePartId.Value).ToList();
						ViewBag.SelectedMachinePartId = machinePartId.Value;
					}
				
					else
					{
						ViewBag.SelectedMachinePartId = null;
					}
				}
				else if (machinePartId == 0)
				{
					values = values.Where(e => e.machinePartId == null).ToList();
					ViewBag.SelectedMachinePartId = 0; 
				}
			}
			if ( machineId == 0) 
			{
				values = values.Where(e => e.machineId == null).ToList();
				ViewBag.SelectedMachineId = 0; 
			}
			else
			{
				if (machinePartId.HasValue && machinePartId.Value > 0)
				{
					values = values.Where(e => e.machinePartId == machinePartId.Value).ToList();
					ViewBag.SelectedMachinePartId = machinePartId.Value;
				}
			}

			ViewBag.MachineParts = partsForViewBag;


			if (isClosed== true)
			{
				values=values.Where(e => e.isClosed == true).ToList();
				ViewBag.IsClosed = isClosed;
			}
			else if (isClosed == false)
			{
				values = values.Where(e => e.isClosed == false).ToList();
				ViewBag.IsClosed = isClosed;
			}

				switch (sortBy)
				{
					case "start":
						values = sortOrder == "desc" ? values.OrderByDescending(x => x.workOrderStartDate).ToList() : values.OrderBy(x => x.workOrderStartDate).ToList();
						break;
					case "end":
						values = sortOrder == "desc" ? values.OrderByDescending(x => x.workOrderEndDate).ToList() : values.OrderBy(x => x.workOrderEndDate).ToList();
						break;
					case "id":
						values = sortOrder == "desc" ? values.OrderByDescending(x => x.id).ToList() : values.OrderBy(x => x.id).ToList();
						break;
					case "machineId":
						values = sortOrder == "desc" ? values.OrderByDescending(x => x.machineId).ToList() : values.OrderBy(x => x.machineId).ToList();
						break;
					case "isClosed":
						values = sortOrder == "desc" ? values.OrderByDescending(x => x.isClosed).ToList() : values.OrderBy(x => x.isClosed).ToList();
						break;
					case "userId":
						values = sortOrder == "desc" ? values.OrderByDescending(x => x.userId).ToList() : values.OrderBy(x => x.userId).ToList();
						break;
			}
			return View(values); 
		}
		[Authorize(Policy = "BasicModeratorEditPermission")]
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
		[Authorize(Policy = "BasicModeratorEditPermission")]
		public IActionResult DeleteWork(int id)
		{
			var workk = workManager.TGetById(id);
			var workOrder = _context.WorkOrders.FirstOrDefault(x => x.id == workk.workOrderId);
			if (workk != null)
			{
				workOrder.workOrderEndDate = null;
				workOrder.isClosed = false;
				workManager.TRemove(workk);

				return RedirectToAction("WorkReportss","Report");
			}
			return NotFound();
		}
		[Authorize(Policy = "BasicModeratorEditPermission")]
		[HttpGet]
		public IActionResult AddWorkOrder()
		{
			var machines = _context.Machines.Select(m => new SelectListItem
			{
				Value = m.id.ToString(),
				Text = m.name.ToString()
			}).ToList();
			ViewBag.MachineList = machines;
			var users = _context.mobileUsers.Select(m => new SelectListItem
			{
				Value = m.Id.ToString(),
				Text = m.UserName.ToString()
			}).ToList();
			ViewBag.UserList = users;
			return View();
		}
		[Authorize(Policy = "BasicModeratorEditPermission")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult AddWorkOrder(workOrder p)
		{
			if(!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Lütfen gerekli alanları doldurunuz.");
				return View(p);
			}
			bool isBlankField=false;
			ViewBag.MachineList = _context.Machines.Select(m => new SelectListItem
			{
				Value = m.id.ToString(),
				Text = m.name
			}).ToList();
			ViewBag.UserList= _context.mobileUsers.Select(m => new SelectListItem
			{
				Value = m.Id.ToString(),
				Text = m.UserName.ToString()
			}).ToList();
			if (p.title == null)
			{
				ModelState.AddModelError("title", "Lütfen bir başlık giriniz.");
				isBlankField = true;
				
			}
			if (p.desc == null)
			{
				ModelState.AddModelError("desc", "Lütfen bir açıklama giriniz.");
				isBlankField = true;
				
			}
			if (p.userId == null)
			{
				ModelState.AddModelError("userId", "Lütfen bir kullanıcı seçiniz.");
				isBlankField = true;
				
			}
			if (isBlankField)
			{
				return View(p);
			}
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
				p.workOrderTempStartDate = p.workOrderStartDate;
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

			return Json(parts); 
		}
		[Authorize(Policy = "BasicModeratorEditPermission")]
		public IActionResult EditWorkOrder(int id)
		{

			var machines = _context.Machines.Select(m => new SelectListItem
			{
				Value = m.id.ToString(),
				Text = m.name.ToString()
			}).ToList();
			ViewBag.MachineList = machines; 
			var users = _context.mobileUsers.Select(m => new SelectListItem
			{
				Value = m.Id.ToString(),
				Text = m.UserName.ToString()
			}).ToList();
			ViewBag.UserList = users;

			var value = workOrderManager.TGetById(id);
			return View(value);
		}
		[Authorize(Policy = "BasicModeratorEditPermission")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EditWorkOrder(workOrder p)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Lütfen gerekli alanları doldurunuz.");
				return View(p);
			}
			var existing = workOrderManager.TGetById(p.id); 

			if (existing == null)
				return NotFound();

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
			var workReports = workManager.TGetList();
			if (id != null && id != 0)
			{
				workReports = workReports.Where(x => x.workOrderId == id).ToList();
			}
			if (workReports.Count != 0)
			{
				return View(workReports);
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
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ExportToExcel([FromBody] List<string> workOrderIds)
		{
			if (workOrderIds.Count > 1000)
			{
				return BadRequest("Excel'e aktarılacak veri sayısı 1000'den fazla olamaz.");
			}
			if (workOrderIds == null || !workOrderIds.Any())
			{
				return BadRequest("Aktarılacak ID listesi boş.");
			}

			try
			{
				var idListesiInt = workOrderIds.Select(int.Parse).ToList();

				var veriler = await _context.WorkOrders 
										 .Where(x => idListesiInt.Contains(x.id))
										 .Include(x => x.machine)
										 .Include(x=> x.machinePart)
										 .ToListAsync();

				var siraliVeriler = veriler
									.OrderBy(v => idListesiInt.IndexOf(v.id)) 
									.ToList();

				using (var workbook = new XLWorkbook())
				{
					var worksheet = workbook.Worksheets.Add("Veriler"); 

					worksheet.Cell(1, 1).Value = "ID";
					worksheet.Cell(1, 2).Value = "Başlık";
					worksheet.Cell(1, 3).Value = "Açıklama";
					worksheet.Cell(1, 4).Value = "İş sonlandı mı";
					worksheet.Cell(1, 5).Value = "İş emri başlangıç tarihi";
					worksheet.Cell(1, 6).Value = "İş emri bitiş tarihi";
					worksheet.Cell(1, 7).Value = "Makine";
					worksheet.Cell(1, 8).Value = "Makine Parçası";

					var headerRange = worksheet.Range("A1:H1"); 
					headerRange.Style.Font.Bold = true;
					headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

					int currentRow = 2; 
					foreach (var veri in siraliVeriler)
					{
						worksheet.Cell(currentRow, 1).Value = veri.id;
						worksheet.Cell(currentRow, 2).Value = veri.title;
						worksheet.Cell(currentRow, 3).Value = veri.desc;
						worksheet.Cell(currentRow, 4).Value = veri.isClosed == true ? "EVET": "HAYIR";
						worksheet.Cell(currentRow, 5).Value = veri.workOrderStartDate;
						worksheet.Cell(currentRow, 6).Value = veri.workOrderEndDate != null ? veri.workOrderEndDate.ToString() : "-";
						worksheet.Cell(currentRow, 7).Value = veri.machine?.name?? "Makine bilgisi yok.";
						worksheet.Cell(currentRow, 8).Value = veri.machinePart?.name?? "Makine parçası bilgisi yok.";

						currentRow++;
					}

					worksheet.Columns().AdjustToContents();

					using (var stream = new MemoryStream())
					{
						workbook.SaveAs(stream);
						var content = stream.ToArray();
						var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
						var fileName = $"veriler_{DateTime.Now:yyyy.MM.dd-HH:mm:ss}.xlsx"; // Dinamik dosya adı

						return File(content, contentType, fileName);
					}
				}
			}
			catch (Exception ex)
			{
				return Problem("Excel dosyası oluşturulurken bir hata oluştu. Detaylar için sunucu loglarını kontrol edin.", statusCode: 500);
			}
		}
		[Authorize(Policy = "BasicModeratorEditPermission")]
		[HttpGet]
		public IActionResult EditWork(int id)
		{
			var machines = _context.Machines.Select(m => new SelectListItem
			{
				Value = m.id.ToString(),
				Text = m.name.ToString()
			}).ToList();
			ViewBag.MachineList = machines; 
			var users = _context.mobileUsers.Select(m => new SelectListItem
			{
				Value = m.Id.ToString(),
				Text = m.UserName.ToString()
			}).ToList();
			ViewBag.UserList = users;

			var value = workManager.TGetById(id);
			return View(value);
		}
		[Authorize(Policy = "BasicModeratorEditPermission")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EditWork(work p)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Lütfen gerekli alanları doldurunuz.");
				return View(p);
			}
			var existing = workManager.TGetById(p.id);
			existing.machineId = p.machineId;
			existing.machinePartId = p.machinePartId;
			existing.title = p.title;	
			existing.desc = p.desc;
			existing.userId = p.userId;
			;
			workManager.TUpdate(existing);
			return RedirectToAction("WorkReportss","Report");
		}
	}
}
