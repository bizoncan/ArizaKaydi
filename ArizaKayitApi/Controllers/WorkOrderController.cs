using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKayitApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WorkOrderController : ControllerBase
	{
		private readonly context _context;

		public WorkOrderController(context context)
		{
			_context = context;
		}
		[HttpGet]
		public IActionResult getWorkOrders()
		{
			var values = _context.WorkOrders.ToList();
			return Ok(values);
		}
		[HttpGet("{id}")]
		public IActionResult getWorkOrder(int id)
		{
			var value = _context.WorkOrders.Find(id);
			return Ok(value);
		}
		[HttpPut]
		public IActionResult updateWorkOrder([FromBody]workOrder w)
		{
			_context.WorkOrders.Update(w);
			_context.SaveChanges();
			return Ok();
		}
		[HttpGet("GetUserId")]
		public IActionResult getUserId(String username)
		{
			var user = _context.Users.FirstOrDefault(u => u.UserName == username);
			if (user != null)
			{

				return Ok(user.Id);
			}

			return Ok(0);
		}
		[HttpGet("GetUserName")]
		public IActionResult getUserName([FromQuery]int userId)
		{
			var user = _context.Users.Find(userId);
			if (user != null)
			{
				return Ok(user.UserName);
			}
			return Ok("Kullanıcı Bulunamadı");
		}
	}
}
