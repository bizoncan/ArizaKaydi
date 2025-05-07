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
		//[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Index(DateTime? startDate = null, DateTime? endDate = null, int? machineId = null, int? machinePartId = null, string? sortBy=null, string? sortOrder=null, Boolean? isClosed=null)
		{
			var values = _context.WorkOrders.Include(x => x.machine).ToList(); // Başlangıçta tüm iş emirlerini al

			ViewBag.Machines = _context.Machines.ToList(); // Makine dropdown'ı için tüm makineler

			// Makine Parçaları için boş bir liste oluştur, sonra dolduracağız
			List<machinePart> partsForViewBag = new List<machinePart>();

			// Başlangıç Tarihi Filtresi
			if (startDate.HasValue)
			{
				values = values.Where(e => e.workOrderStartDate >= startDate.Value).ToList();
				ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
			}

			// Bitiş Tarihi Filtresi (Tüm günü kapsayacak şekilde)
			if (endDate.HasValue)
			{
				var endOfDay = endDate.Value.Date.AddDays(1).AddSeconds(-1);
				// Bitiş tarihi olan ve belirtilen aralıkta olanları filtrele
				values = values.Where(e => e.workOrderStartDate <= endOfDay).ToList();
				ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
			}

			// Makine Filtresi
			if (machineId.HasValue && machineId.Value > 0)
			{
				values = values.Where(e => e.machineId == machineId.Value).ToList();
				ViewBag.SelectedMachineId = machineId.Value;

				// *** DÜZELTME: Sadece seçili makineye ait parçaları ViewBag'e ata ***
				partsForViewBag = _context.MachineParts
									  .Where(mp => mp.machineId == machineId.Value)
									  .ToList();

				// Makine Parçası Filtresi (Makine seçiliyse burada yapılır)
				if (machinePartId.HasValue && machinePartId.Value > 0)
				{
					// Güvenlik kontrolü: Seçilen parça gerçekten seçilen makineye mi ait?
					if (partsForViewBag.Any(p => p.Id == machinePartId.Value))
					{
						values = values.Where(e => e.machinePartId == machinePartId.Value).ToList();
						ViewBag.SelectedMachinePartId = machinePartId.Value;
					}
					else
					{
						// Eğer parça o makineye ait değilse, parça filtresini temizle (opsiyonel)
						ViewBag.SelectedMachinePartId = null;
					}
				}
			}
			else // Makine seçilmemişse ("Tümü")
			{
				// Makine seçilmeden parça seçilmişse (JavaScript bunu engellemeli ama yine de kontrol edilebilir)
				if (machinePartId.HasValue && machinePartId.Value > 0)
				{
					values = values.Where(e => e.machinePartId == machinePartId.Value).ToList();
					ViewBag.SelectedMachinePartId = machinePartId.Value;
					// partsForViewBag boş kalır, bu doğru.
				}
			}

			// Filtrelenmiş (veya boş) parça listesini ViewBag'e ata
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
			}
			return View(values); // Filtrelenmiş iş emirlerini View'a gönder
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
			var users = _context.mobileUsers.Select(m => new SelectListItem
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
			var users = _context.mobileUsers.Select(m => new SelectListItem
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
			var workReports = workManager.TGetList();
			if (id != null && id != 0)
			{
				workReports = workReports.Where(x => x.workOrderId == id).ToList();
			}
			if (workReports.Count != 0)
			{
				//var works = workManager.TGetWorkByWorkOrderList(id);
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
		[ValidateAntiForgeryToken] // CSRF koruması için (eğer token gönderdiyseniz)
		public async Task<IActionResult> ExportToExcel([FromBody] List<string> orderedIds) // Gelen ID listesi (tipi int ise List<int> yapın)
		{
			if (orderedIds == null || !orderedIds.Any())
			{
				return BadRequest("Aktarılacak ID listesi boş.");
			}

			try
			{
				// ID'leri int'e çevirme (eğer veritabanı ID'leriniz int ise)
				var idListesiInt = orderedIds.Select(int.Parse).ToList();

				// 1. Veritabanından verileri çekme (Örnek - Entity Framework Core ile)
				// Not: Tüm veriyi çekip sonra sıralamak yerine, veritabanında WHERE IN ile filtrelemek daha performanslıdır.
				var veriler = await _context.WorkOrders // Kendi DbContext ve DbSet adınızı kullanın
										 .Where(x => idListesiInt.Contains(x.id))
										 .Include(x => x.machine)
										 .Include(x=> x.machinePart)
										 .ToListAsync();

				// 2. Verileri istemciden gelen sıraya göre SIRALAMA (Çok Önemli!)
				var siraliVeriler = veriler
									.OrderBy(v => idListesiInt.IndexOf(v.id)) // Gelen listedeki index'e göre sırala
									.ToList();

				// 3. Excel Dosyası Oluşturma (ClosedXML ile)
				using (var workbook = new XLWorkbook())
				{
					var worksheet = workbook.Worksheets.Add("Veriler"); // Sayfa adı

					// Başlık Satırını Ekleme (Örnek)
					worksheet.Cell(1, 1).Value = "ID";
					worksheet.Cell(1, 2).Value = "Başlık";
					worksheet.Cell(1, 3).Value = "Açıklama";
					worksheet.Cell(1, 4).Value = "İş sonlandı mı";
					worksheet.Cell(1, 5).Value = "İş emri başlangıç tarihi";
					worksheet.Cell(1, 6).Value = "İş emri bitiş tarihi";
					worksheet.Cell(1, 7).Value = "Makine";
					worksheet.Cell(1, 8).Value = "Makine Parçası";

					// Başlık satırını biçimlendirme (Opsiyonel)
					var headerRange = worksheet.Range("A1:H1"); // Başlıkların olduğu aralık
					headerRange.Style.Font.Bold = true;
					headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

					// Veri Satırlarını Ekleme
					int currentRow = 2; // Başlıktan sonraki satır
					foreach (var veri in siraliVeriler)
					{
						worksheet.Cell(currentRow, 1).Value = veri.id;
						worksheet.Cell(currentRow, 2).Value = veri.title;
						worksheet.Cell(currentRow, 3).Value = veri.desc;
						worksheet.Cell(currentRow, 4).Value = veri.isClosed == true ? "EVET": "HAYIR";
						worksheet.Cell(currentRow, 5).Value = veri.workOrderStartDate;
						worksheet.Cell(currentRow, 6).Value = veri.workOrderEndDate;
						worksheet.Cell(currentRow, 7).Value = veri.machine?.name;
						worksheet.Cell(currentRow, 8).Value = veri.machinePart?.name;

						// ... Diğer özellikler
						currentRow++;
					}

					// Sütun genişliklerini ayarlama (Opsiyonel)
					worksheet.Columns().AdjustToContents();

					// 4. Excel Dosyasını MemoryStream'e Kaydetme
					using (var stream = new MemoryStream())
					{
						workbook.SaveAs(stream);
						var content = stream.ToArray();
						var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
						var fileName = $"veriler_{DateTime.Now:yyyy.MM.dd-HH:mm:ss}.xlsx"; // Dinamik dosya adı

						// 5. Dosyayı İstemciye Gönderme
						return File(content, contentType, fileName);
					}
				}
			}
			catch (Exception ex)
			{
				// Hata loglama mekanizmanızı burada kullanın
				// Log.Error(ex, "Excel dışa aktarma sırasında hata oluştu.");
				// İstemciye genel bir hata mesajı dönebilirsiniz
				// return StatusCode(500, "Excel dosyası oluşturulurken sunucu tarafında bir hata oluştu.");
				return Problem("Excel dosyası oluşturulurken bir hata oluştu. Detaylar için sunucu loglarını kontrol edin.", statusCode: 500);
			}
		}
	}
}
