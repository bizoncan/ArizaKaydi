using BusinessLayer.Concrete;
using ClosedXML.Excel;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using DocumentFormat.OpenXml.Spreadsheet;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ArizaKaydi.Controllers
{
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
			var values = _context.Works.Include(x => x.machine).ToList();
			ViewBag.Machines = _context.Machines.ToList(); // Makine dropdown'ı için tüm makineler
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
				ViewBag.SelectedUserId = null; // Seçili kullanıcı yoksa null yap
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
				else if (machinePartId == 0)
				{
					values = values.Where(e => e.machinePartId == null).ToList();
					ViewBag.SelectedMachinePartId = 0; // Seçili parça yoksa null yap
				}
			}
			if (machineId == 0) // Makine seçilmemişse
			{
				// Makine seçilmediğinde tüm parçaları göster
				values = values.Where(e => e.machineId == null).ToList();
				ViewBag.SelectedMachineId = 0; // Seçili makine yoksa null yap
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

			return Json(parts); // ← Tarayıcıya JSON veri gönder
		}
		[HttpPost]
		[ValidateAntiForgeryToken] // CSRF koruması için (eğer token gönderdiyseniz)
		public async Task<IActionResult> ExportToExcel([FromBody] List<string> reportIds) // Gelen ID listesi (tipi int ise List<int> yapın)
		{
			if (reportIds == null || !reportIds.Any())
			{
				return BadRequest("Aktarılacak ID listesi boş.");
			}

			try
			{
				// ID'leri int'e çevirme (eğer veritabanı ID'leriniz int ise)
				var idListesiInt = reportIds.Select(int.Parse).ToList();

				// 1. Veritabanından verileri çekme (Örnek - Entity Framework Core ile)
				// Not: Tüm veriyi çekip sonra sıralamak yerine, veritabanında WHERE IN ile filtrelemek daha performanslıdır.
				var veriler = await _context.Works // Kendi DbContext ve DbSet adınızı kullanın
										 .Where(x => idListesiInt.Contains(x.id))
										 .Include(x => x.machine)
										 .Include(x => x.machinePart)
										 .Include(x => x.userI)
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
					
					worksheet.Cell(1, 4).Value = "İş emri başlangıç tarihi";
					worksheet.Cell(1, 5).Value = "İş emri bitiş tarihi";
					worksheet.Cell(1, 6).Value = "Makine";
					worksheet.Cell(1, 7).Value = "Makine Parçası";
					worksheet.Cell(1, 8).Value = "Raporu giren kullanıcı";
					worksheet.Cell(1, 9).Value = "İş emri id'si";
					worksheet.Cell(1, 10).Value = "Geçmiş Rapor mu";


					// Başlık satırını biçimlendirme (Opsiyonel)
					var headerRange = worksheet.Range("A1:J1"); // Başlıkların olduğu aralık
					headerRange.Style.Font.Bold = true;
					headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

					// Veri Satırlarını Ekleme
					int currentRow = 2; // Başlıktan sonraki satır
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
						var fileName = $"raporlar_{DateTime.Now:yyyy.MM.dd-HH:mm:ss}.xlsx"; // Dinamik dosya adı

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
