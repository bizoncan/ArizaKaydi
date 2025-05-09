using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKaydi.Controllers
{
	[AllowAnonymous]
	public class ErrorPageController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult AccessDenied()
		{
			return View();
		}
		public IActionResult TheErrorView()
		{
			return View();
		}
	}
}
