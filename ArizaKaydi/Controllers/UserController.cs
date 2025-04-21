using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKaydi.Controllers
{
	public class UserController : Controller
	{
		UserManager _userManager;
		context _context;
		public UserController(context context)
		{
			
			_context = context;
			_userManager = new UserManager(new EFUserDal(_context));
		}
		public IActionResult Index()
		{
			var values = _userManager.TGetList();
			return View(values);
		}
		public IActionResult RemoveUser(int id)
		{
			var user = _userManager.TGetById(id);
			_userManager.TRemove(user);
			return RedirectToAction("Index");
		}
		public IActionResult EditUser(int id)
		{
			var user = _userManager.TGetById(id);
			return View(user);
		}
		[HttpPost]
		public IActionResult EditUser(user u)
		{
			// Email geçerlilik kontrolü  
			if (!IsValidEmail(u.Email))
			{
				ModelState.AddModelError("Email", "Geçerli bir email adresi giriniz.");
				return View(u);
			}

			// Kullanıcıyı veritabanından al  
			var existingUser = _userManager.TGetById(u.Id);

			// Eğer kullanıcı bulunamazsa hata döndür  
			if (existingUser == null)
			{
				ModelState.AddModelError("", "Kullanıcı bulunamadı.");
				return View(u);
			}

			// Kullanıcı adı veya email başka bir kullanıcıya ait mi?  
			var duplicateUser = _userManager.TGetList()
										.FirstOrDefault(x => (x.UserName == u.UserName || x.Email == u.Email) && x.Id != u.Id);

			if (duplicateUser != null)
			{
				if (duplicateUser.UserName == u.UserName)
					ModelState.AddModelError("Username", "Bu kullanıcı adı zaten alınmış.");

				if (duplicateUser.Email == u.Email)
					ModelState.AddModelError("Email", "Bu email adresi zaten kayıtlı.");

				return View(u);
			}

			// Mevcut kullanıcı bilgilerini güncelle  
			existingUser.UserName = u.UserName;
			existingUser.Email = u.Email;
			existingUser.PasswordHash = existingUser.PasswordHash;

			_userManager.TUpdate(existingUser);
			return RedirectToAction("Index");
		}
		public IActionResult AddUser()
		{
			return View();
		}
		[HttpPost]
		public IActionResult AddUser(user u)
		{
			// Email geçerlilik kontrolü
			if (!IsValidEmail(u.Email))
			{
				ModelState.AddModelError("Email", "Geçerli bir email adresi giriniz.");
				return View(u);
			}

			// Username veya email veritabanında var mı?
			var existingUser = _userManager.TGetList()
				.FirstOrDefault(x => x.UserName == u.UserName || x.Email == u.Email);

			if (existingUser != null)
			{
				if (existingUser.UserName == u.UserName)
					ModelState.AddModelError("Username", "Bu kullanıcı adı zaten alınmış.");

				if (existingUser.Email == u.Email)
					ModelState.AddModelError("Email", "Bu email adresi zaten kayıtlı.");

				return View(u);
			}

			// Şifreyi hashle
			var passwordHasher = new PasswordHasher<user>();
			u.PasswordHash = passwordHasher.HashPassword(u, u.PasswordHash);

			// Kullanıcıyı kaydet
			_userManager.TAdd(u);
			return RedirectToAction("Index");
		}

		// Email geçerlilik kontrolü yapan yardımcı metot
		private bool IsValidEmail(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}
	}
}
