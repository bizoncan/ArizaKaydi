using ArizaKaydi.Models;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKaydi.Controllers
{
	[AllowAnonymous]
	public class RegisterController : Controller
	{
		private readonly UserManager<panelUser> _userManager;

		public RegisterController(UserManager<panelUser> userManager)
		{
			_userManager = userManager;
		}
		[HttpGet]
		public IActionResult Index()
		{ 	
			return View(new PanelUserRegisterViewModel());
		}
		[HttpPost]
		public async Task<IActionResult> Index(PanelUserRegisterViewModel model)
		{
			var u = new panelUser()
			{
				Name = model.Name,
				Surname = model.Surname,
				UserName = model.UserName,
				Email = model.Email
			};
			if(ModelState.IsValid == false)
			{
				return View(model);
			}
			if (model.Password == model.ConfirmPassword)
			{
				var result = await _userManager.CreateAsync(u, model.Password);
				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Login");
				}
				else
				{
					foreach (var item in result.Errors)
					{
						ModelState.AddModelError("", item.Description);
					}
				}
			}
			else
			{
				ModelState.AddModelError("", "Şifreler uyuşmuyor.");
			}
			return View();
		}
	}
}
