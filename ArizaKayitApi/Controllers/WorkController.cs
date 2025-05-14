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
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = "İş kaydı oluşturulurken bir hata oluştu", error = ex.Message });
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
