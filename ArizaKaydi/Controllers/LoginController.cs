using ArizaKaydi.Models;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKaydi.Controllers
{
	[AllowAnonymous]
	public class LoginController : Controller
	{

		private readonly SignInManager<panelUser> _signInManager;

		public LoginController(SignInManager<panelUser> signInManager)
		{
			_signInManager = signInManager;
		}
		[HttpGet]
		public IActionResult Index()
		{

			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Index(PanelUserLoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, true);
				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
				}
			}
			return View(model);
		}
		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Login");
		}
	}
}
