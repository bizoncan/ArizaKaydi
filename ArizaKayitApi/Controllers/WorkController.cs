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
			_context.Works.Add(w);
			_context.SaveChanges();
			return Ok();
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
