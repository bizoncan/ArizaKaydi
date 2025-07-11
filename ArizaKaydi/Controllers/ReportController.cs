using BusinessLayer.Concrete;
using ClosedXML.Excel;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using DocumentFormat.OpenXml.Spreadsheet;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ArizaKaydi.Controllers
{
	[Authorize(Policy = "BasicModeratorViewPermission")]
	public class ReportController : Controller
	{
		WorkManager workManager;
		WorkOrderManager workOrderManager;
		ImageCollectionManager imageCollectionManager;
		context _context;

		public ReportController(context context)
		{
			_context = context;
			this.workManager = new WorkManager(new EFWorkDal(_context));
			this.workOrderManager = new WorkOrderManager(new EFWorkOrderDal(_context));
			this.imageCollectionManager = new ImageCollectionManager(new EFImageCollectionDal(_context));

		}

		public IActionResult Index()
		{
		
			return View();
		}
		public IActionResult WorkReportss(DateTime? startDate = null, DateTime? endDate = null, int? machineId = null, int? machinePartId = null, string? sortBy=null, string? sortOrder=null, int? userId=null)
		{
			var values = _context.Works.Include(x => x.machine).Include(x=>x.userI).ToList();
			ViewBag.Machines = _context.Machines.ToList(); 
			List<machinePart> partsForViewBag = new List<machinePart>();
			ViewBag.MobileUsers = _context.mobileUsers.ToList();
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

			if (userId.HasValue && userId.Value > 0)
			{
				values = values.Where(e => e.userId == userId.Value).ToList();
				ViewBag.SelectedUserId = userId.Value;
			}
			else
			{
				ViewBag.SelectedUserId = null; 
			}

			
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
			if (machineId == 0) 
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
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ExportToExcel([FromBody] List<string> reportIds)
		{
			if (reportIds == null || !reportIds.Any())
			{
				return BadRequest("Aktarılacak ID listesi boş.");
			}

			try
			{
				var idListesiInt = reportIds.Select(int.Parse).ToList();

				var veriler = await _context.Works 
										 .Where(x => idListesiInt.Contains(x.id))
										 .Include(x => x.machine)
										 .Include(x => x.machinePart)
										 .Include(x => x.userI)
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
					
					worksheet.Cell(1, 4).Value = "İş emri başlangıç tarihi";
					worksheet.Cell(1, 5).Value = "İş emri bitiş tarihi";
					worksheet.Cell(1, 6).Value = "Makine";
					worksheet.Cell(1, 7).Value = "Makine Parçası";
					worksheet.Cell(1, 8).Value = "Raporu giren kullanıcı";
					worksheet.Cell(1, 9).Value = "İş emri id'si";
					worksheet.Cell(1, 10).Value = "Geçmiş Rapor mu";


					var headerRange = worksheet.Range("A1:J1"); 
					headerRange.Style.Font.Bold = true;
					headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

					int currentRow = 2; 
					foreach (var veri in siraliVeriler)
					{
						worksheet.Cell(currentRow, 1).Value = veri.id;
						worksheet.Cell(currentRow, 2).Value = veri.title;
						worksheet.Cell(currentRow, 3).Value = veri.desc;
						worksheet.Cell(currentRow, 4).Value = veri.workOrderStartDate;
						worksheet.Cell(currentRow, 5).Value = veri.workOrderEndDate;
						worksheet.Cell(currentRow, 6).Value = veri.machine?.name?? "Makine bilgisi yok";
						worksheet.Cell(currentRow, 7).Value = veri.machinePart?.name?? "Makine parçası bilgisi yok.";
						worksheet.Cell(currentRow, 8).Value = veri.userI?.UserName?? "Kullanıcı bilgisi yok.";
						worksheet.Cell(currentRow, 9).Value = veri.workOrderId;
						worksheet.Cell(currentRow, 10).Value = veri.isPastWork == true  ? "Evet":"Hayır";

						currentRow++;
					}

					worksheet.Columns().AdjustToContents();

					using (var stream = new MemoryStream())
					{
						workbook.SaveAs(stream);
						var content = stream.ToArray();
						var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
						var fileName = $"raporlar_{DateTime.Now:yyyy.MM.dd-HH:mm:ss}.xlsx"; 

						return File(content, contentType, fileName);
					}
				}
			}
			catch (Exception ex)
			{
				return Problem("Excel dosyası oluşturulurken bir hata oluştu. Detaylar için sunucu loglarını kontrol edin.", statusCode: 500);
			}
		}
	}
}
