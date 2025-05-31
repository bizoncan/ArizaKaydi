using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKaydi.Controllers
{
	public class AiTestController : Controller
	{
		[Authorize(Policy = "BasicModeratorViewPermission")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
