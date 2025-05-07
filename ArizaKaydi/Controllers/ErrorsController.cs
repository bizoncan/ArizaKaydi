using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using Microsoft.AspNetCore.Mvc.Rendering;

using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using ArizaKaydi.Models;
using Microsoft.EntityFrameworkCore;
namespace ArizaKaydi.Controllers
{
    public class ErrorsController : Controller
    {
        ErrorManager errorManager;
        ImageCollectionManager imageCollectionManager;
        private readonly context _context;
        
		public ErrorsController(ErrorManager errorManager, ImageCollectionManager imageCollectionManager, context context)
		{
			_context = context;
            this.errorManager = errorManager;
			this.imageCollectionManager = imageCollectionManager;
		}

		public IActionResult Index(int id, DateTime? startDate = null, DateTime? endDate = null, int? machinePartId = null)
        {

			ViewBag.MachineParts = _context.MachineParts.Where(p=> p.machineId == id).ToList();
			var p = _context.Errors.Include(e => e.machinePartName).Where(x => x.machineId == id).Select(x => new error
			{
				id = x.id,
				errorType = x.errorType,
				errorDate = x.errorDate,
				machineId = x.machineId,
				machinePartName = x.machinePartName,
				machinePartId = x.machinePartId,
			}).ToList();
			if (startDate.HasValue)
			{
				p = p.Where(e => e.errorDate >= startDate.Value).ToList();
				ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
			}

			if (endDate.HasValue)
			{
				p = p.Where(e => e.errorDate <= endDate.Value.AddDays(1).AddSeconds(-1)).ToList();
				ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
			}
			if (machinePartId.HasValue && machinePartId.Value > 0)
			{
				p = p.Where(e => e.machinePartId == machinePartId.Value).ToList();
				ViewBag.SelectedMachinePartId = machinePartId.Value;
			}

			return View(p);
        }

        [HttpGet]
        public IActionResult AddError()
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
        public IActionResult AddError(error p)
        {
            errorManager.TAdd(p);
            return RedirectToAction("Index", "Default");
        }
        public IActionResult DeleteError(int id)
        {
            var value = errorManager.TGetById(id);
            errorManager.TRemove(value);
            return RedirectToAction("Index", "Default");
        }
        [HttpGet]
        public IActionResult EditError(int id)
        {

            var machines = _context.Machines.Select(m => new SelectListItem
            {
                Value = m.id.ToString(),
                Text = m.name.ToString()
            }).ToList();
            ViewBag.MachineList = machines; // Use the same name as in AddError for consistency


            var value = errorManager.TGetById(id);
            return View(value);
        }
        [HttpGet]
        public IActionResult ErrorDetail(int id)
        {
            var errors = errorManager.TGetMachineInfos(id);
			var images = imageCollectionManager.TGetImageCollectionByErrorId(id);
            List<string> imageList= new List<string>();
            if(errors.errorImageBytes != null)
			{
                string _imreBase64Data = Convert.ToBase64String(errors.errorImageBytes);
				imageList.Add(string.Format("data:image/jpeg;base64,{0}", _imreBase64Data));
			}
			if (images.Count>0)
			{
                string imreBase64Data;
				foreach (var item in images)
				{
                    imreBase64Data =  Convert.ToBase64String(item.imageDataByte);
					imageList.Add(string.Format("data:image/jpeg;base64,{0}", imreBase64Data));
				}
			}
			var er_im = new ErrorDetailModel
            {
				errorInfo = errors,
                collection = imageList
			};


            return View(er_im);
        }
        [HttpPost]
        public IActionResult EditError(error p)
        {
            errorManager.TUpdate(p);
            return RedirectToAction("Index", "Default");
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
