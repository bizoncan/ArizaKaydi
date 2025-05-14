using ArizaKaydi.Models;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO; // Dosya işlemleri için
using System.Threading.Tasks; // Task için
using Microsoft.AspNetCore.Hosting; // IWebHostEnvironment için (opsiyonel, dosya yolu için)

namespace ArizaKaydi.Controllers
{
	public class PanelUserController : Controller
	{
		private readonly UserManager<panelUser> _userManager;
		private readonly IWebHostEnvironment _webHostEnvironment; // Resim kaydetme yolu için
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
				// Kullanıcı bulunamazsa bir hata sayfasına yönlendirebilirsiniz
				TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
				return RedirectToAction("Index", "Home");
			}

			PanelUserEditViewModel panelUserViewModel = new PanelUserEditViewModel
			{
				Name = user.Name,
				Surname = user.Surname,
				ImageURL = user.ImageURL, // Mevcut resmi göstermek için
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
				// ViewModel geçerli değilse, hatalarla birlikte formu tekrar göster
				return View(panelUserViewModel);
			}

			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			panelUserViewModel.ImageURL = user.ImageURL;
			if (user == null)
			{
				TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
				return RedirectToAction("Index", "Home");
			}

			// Şifre Güncelleme Mantığı
			bool passwordChangedSuccessfully = true; // Şifre değiştirme işlemi yapılmadıysa veya başarılıysa true kalır
			if (!string.IsNullOrEmpty(panelUserViewModel.Password)) // Yeni şifre girilmişse
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
					// Mevcut şifreyi kontrol et ve yeni şifreyi ayarla
					var changePasswordResult = await _userManager.ChangePasswordAsync(user, panelUserViewModel.passwordNow, panelUserViewModel.Password);
					if (changePasswordResult.Succeeded)
					{
						isPasswordC = true;
					}
					if (!changePasswordResult.Succeeded)
					{
						foreach (var error in changePasswordResult.Errors)
						{
							// Genel bir hata veya mevcut şifre hatası olabilir.
							// Identity genellikle "Incorrect password." gibi bir mesaj döner.
							ModelState.AddModelError(string.Empty, error.Description);
						}
						passwordChangedSuccessfully = false;
					}
				}
			}

			if (!passwordChangedSuccessfully) // Şifre ile ilgili bir hata varsa formu tekrar göster
			{
				return View(panelUserViewModel);
			}
			if (panelUserViewModel.ImageFile != null && panelUserViewModel.ImageFile.Length > 0)
			{
				if (panelUserViewModel.ImageFile.Length > 2 * 1024 * 1024) // Örnek: 2MB limit
				{
					ModelState.AddModelError("ImageFile", "Dosya boyutu 2MB'den büyük olamaz.");
					if (!string.IsNullOrEmpty(user.ImageURL))
					{ // Mevcut resmi koru
						panelUserViewModel.ImageURL = user.ImageURL;
					}
					return View(panelUserViewModel);
				}

				// Desteklenen dosya türlerini kontrol et
				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
				var extension = Path.GetExtension(panelUserViewModel.ImageFile.FileName).ToLowerInvariant();
				if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
				{
					ModelState.AddModelError("ImageFile", "Geçersiz dosya türü. Sadece JPG, JPEG, PNG, GIF kabul edilir.");
					if (!string.IsNullOrEmpty(user.ImageURL))
					{ // Mevcut resmi koru
						panelUserViewModel.ImageURL = user.ImageURL;
					}
					return View(panelUserViewModel);
				}


				using (var memoryStream = new MemoryStream())
				{
					await panelUserViewModel.ImageFile.CopyToAsync(memoryStream);
					byte[] imageBytes = memoryStream.ToArray();
					string base64String = Convert.ToBase64String(imageBytes);
					// Data URI scheme: data:[<MIME-type>][;charset=<encoding>][;base64],<data>
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
				// Kullanıcı bilgilerini (özellikle adı) güncelledikten sonra oturumu yenilemek gerekebilir.
				if(isUserC || isPasswordC)
				{
					await _signInManager.RefreshSignInAsync(user); // Eğer SignInManager enjekte edilmişse
					return RedirectToAction("Index", "Login");
				}
				else
				{
					return RedirectToAction("PanelUserSetting");
				}
				// Aynı sayfada kalıp mesajı göstermek için	
				// veya return RedirectToAction("Index", "Home");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
			// Güncelleme başarısızsa mevcut ImageURL'i viewmodel'e geri yükle
			panelUserViewModel.ImageURL = user.ImageURL;
			// TempData["ErrorMessage"] = "Bilgiler güncellenirken bir hata oluştu."; // ModelState hataları daha spesifik olacaktır.
			return View(panelUserViewModel);
		}
	}
}