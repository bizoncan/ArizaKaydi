using ArizaKaydi.Models;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO; 
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization; 

namespace ArizaKaydi.Controllers
{
	[Authorize(Policy = "BasicModeratorViewPermission")]
	public class PanelUserController : Controller
	{
		private readonly UserManager<panelUser> _userManager;
		private readonly IWebHostEnvironment _webHostEnvironment; 
		private readonly SignInManager<panelUser> _signInManager;
		public PanelUserController(UserManager<panelUser> userManager, IWebHostEnvironment webHostEnvironment, SignInManager<panelUser> signInManager)
		{
			_userManager = userManager;
			_webHostEnvironment = webHostEnvironment;
			_signInManager = signInManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> PanelUserSetting()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user == null)
			{
				TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
				return RedirectToAction("Index", "Home");
			}

			PanelUserEditViewModel panelUserViewModel = new PanelUserEditViewModel
			{
				Name = user.Name,
				Surname = user.Surname,
				ImageURL = user.ImageURL,
				UserName = user.UserName,
			};
			return View(panelUserViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> PanelUserSetting(PanelUserEditViewModel panelUserViewModel)
		{
			bool isUserC = false;
			bool isPasswordC = false;
			if (!ModelState.IsValid)
			{
				return View(panelUserViewModel);
			}

			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			panelUserViewModel.ImageURL = user.ImageURL;
			if (user == null)
			{
				TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
				return RedirectToAction("Index", "Home");
			}

			bool passwordChangedSuccessfully = true;
			if (!string.IsNullOrEmpty(panelUserViewModel.Password))
			{
				if (string.IsNullOrEmpty(panelUserViewModel.passwordNow))
				{
					ModelState.AddModelError("passwordNow", "Yeni şifre belirlemek için mevcut şifrenizi girmelisiniz.");
					passwordChangedSuccessfully = false;
				}
				else if (panelUserViewModel.Password != panelUserViewModel.ConfirmPassword)
				{
					ModelState.AddModelError("ConfirmPassword", "Yeni şifre ile onay şifresi eşleşmiyor.");
					passwordChangedSuccessfully = false;
				}
				else
				{
					
					var changePasswordResult = await _userManager.ChangePasswordAsync(user, panelUserViewModel.passwordNow, panelUserViewModel.Password);
					if (changePasswordResult.Succeeded)
					{
						isPasswordC = true;
					}
					if (!changePasswordResult.Succeeded)
					{
						foreach (var error in changePasswordResult.Errors)
						{
					
							ModelState.AddModelError(string.Empty, error.Description);
						}
						passwordChangedSuccessfully = false;
					}
				}
			}

			if (!passwordChangedSuccessfully)
			{
				return View(panelUserViewModel);
			}
			if (panelUserViewModel.ImageFile != null && panelUserViewModel.ImageFile.Length > 0)
			{
				if (panelUserViewModel.ImageFile.Length > 2 * 1024 * 1024) 
				{
					ModelState.AddModelError("ImageFile", "Dosya boyutu 2MB'den büyük olamaz.");
					if (!string.IsNullOrEmpty(user.ImageURL))
					{ 
						panelUserViewModel.ImageURL = user.ImageURL;
					}
					return View(panelUserViewModel);
				}

			
				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
				var extension = Path.GetExtension(panelUserViewModel.ImageFile.FileName).ToLowerInvariant();
				if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
				{
					ModelState.AddModelError("ImageFile", "Geçersiz dosya türü. Sadece JPG, JPEG, PNG, GIF kabul edilir.");
					if (!string.IsNullOrEmpty(user.ImageURL))
					{ 
						panelUserViewModel.ImageURL = user.ImageURL;
					}
					return View(panelUserViewModel);
				}


				using (var memoryStream = new MemoryStream())
				{
					await panelUserViewModel.ImageFile.CopyToAsync(memoryStream);
					byte[] imageBytes = memoryStream.ToArray();
					string base64String = Convert.ToBase64String(imageBytes);
					user.ImageURL = $"data:{panelUserViewModel.ImageFile.ContentType};base64,{base64String}";
				}
			}

			user.Name = panelUserViewModel.Name;
			user.Surname = panelUserViewModel.Surname;
			if (user.UserName != panelUserViewModel.UserName)
			{
				isUserC = true;
			}
			user.UserName = panelUserViewModel.UserName;
		
			IdentityResult result = await _userManager.UpdateAsync(user);

			if (result.Succeeded)
			{
				TempData["SuccessMessage"] = "Bilgileriniz başarıyla güncellendi.";
				if(isUserC || isPasswordC)
				{
					await _signInManager.RefreshSignInAsync(user); 
					return RedirectToAction("Index", "Login");
				}
				else
				{
					return RedirectToAction("PanelUserSetting");
				}
			
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
			panelUserViewModel.ImageURL = user.ImageURL;
			return View(panelUserViewModel);
		}
	}
}