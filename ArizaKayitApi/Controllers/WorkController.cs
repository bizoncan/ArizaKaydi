using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKayitApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WorkController : ControllerBase
	{
		private readonly context _context;
		public WorkController(context context)
		{
			_context = context;
		}
		[HttpGet]
		public IActionResult getWorks()
		{
			var values = _context.Works.ToList();

			return Ok(values);
		}
		[HttpGet("GetWork")]
		public IActionResult getWork([FromQuery]int id)
		{
			var value = _context.Works.Where(e=> e.workOrderId == id).Where(e=> e.isOpened == true).FirstOrDefault();
			if (value != null)
			{
				return Ok(value);
			}
			else
			{
				return NotFound();
			}
		}
		[HttpPost]
		public IActionResult addWork([FromBody] work w)
		{
			SetPastWork(w.workOrderId);


			if (_context.WorkOrders.Where(e => e.id == w.workOrderId).FirstOrDefault() == null)
			{
				return NotFound();
			}

			try
			{
				_context.Works.Add(w);
				_context.SaveChanges();
				return Ok(w.id);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = "İş kaydı oluşturulurken bir hata oluştu", error = ex.Message });
			}
		}
		[HttpPost("UpdateWork")]
		public IActionResult updateWork([FromBody] work workModel)
		{
			if (_context.Works.Where(e => e.id == workModel.id).FirstOrDefault() != null)
			{
				_context.ChangeTracker.Clear();
				_context.Works.Update(workModel);
				_context.SaveChanges();
				return Ok(workModel.id);
			}
			else
			{
				return NotFound();
			}
				
		}
		private void SetPastWork(int id)
		{
			var work = _context.Works.Where(x => x.workOrderId == id && x.isPastWork == false).FirstOrDefault();
			if (work == null)
			{
				return;
			}
			work.isPastWork = true;
			_context.Works.Update(work);
			_context.SaveChanges();
		}
	}
}
