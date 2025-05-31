using ArizaKaydi.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using DocumentFormat.OpenXml.Spreadsheet;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ArizaKaydi.Controllers
{
	[Authorize(Policy = "BasicModeratorViewPermission")]
	public class UserController : Controller
	{
		private readonly UserManager<panelUser> panelUserManager;
		private readonly RoleManager<panelUserRole> roleManager;
		private readonly SignInManager<panelUser> signInManager;
		MobileUserManager _userManager;
		context _context;
		public UserController(context context, UserManager<panelUser> panelUserManager, RoleManager<panelUserRole> roleManager, SignInManager<panelUser> signInManager)
		{
			this.panelUserManager = panelUserManager;
			_context = context;
			_userManager = new MobileUserManager(new EFMobileUserDal(_context));
			this.roleManager = roleManager;
			this.signInManager = signInManager;
		}
		[Authorize(Policy = "CanViewMobileUserManagement")]
		public IActionResult Index()
		{
			var values = _userManager.TGetList();
			List<MobileUserViewModel> userViewModels = new List<MobileUserViewModel>();

			int tCount = 0;
			foreach (user u in values)
			{
				tCount = _context.Works.Where(x => x.userId == u.Id).Count();
				MobileUserViewModel viewModel = new MobileUserViewModel();
				viewModel.mobileUser = u;
				viewModel.workCount = tCount;
				userViewModels.Add(viewModel);
			}
			return View(userViewModels);
		}
		[Authorize(Policy = "CanEditMobileUserManagement")]
		public IActionResult RemoveUser(int id)
		{
			var user = _userManager.TGetById(id);
			_userManager.TRemove(user);
			return RedirectToAction("Index");
		}
		[Authorize(Policy = "CanEditMobileUserManagement")]
		public IActionResult EditUser(int id)
		{
			var user = _userManager.TGetById(id);
			return View(user);
		}
		[Authorize(Policy = "CanEditMobileUserManagement")]
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
		[Authorize(Policy = "CanEditMobileUserManagement")]
		public IActionResult AddUser()
		{
			return View();
		}
		[Authorize(Policy = "CanEditMobileUserManagement")]
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
		[Authorize(Policy = "CanViewMobileUserManagement")]
		public IActionResult ChoseUserType()
		{

			return View();
		}
		[Authorize(Policy = "CanViewPanelUserManagement")]
		public async Task<IActionResult> AssignRole()
		{
			var users = await panelUserManager.Users.ToListAsync();
			var userRolesViewModelList = new List<UserRolesViewModel>();
			foreach (panelUser user in users)
			{
				var thisViewModel = new UserRolesViewModel();
				thisViewModel.UserId = user.Id.ToString();
				thisViewModel.Email = user.Email;
				thisViewModel.UserName = user.UserName;
				thisViewModel.Name = user.Name;
				thisViewModel.Surname = user.Surname;
				thisViewModel.Roles = await GetUserRoles(user);
				userRolesViewModelList.Add(thisViewModel);
			}
			ViewBag.Roles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
			return View(userRolesViewModelList);
		}
		[Authorize(Policy = "CanEditPanelUserManagement")]
		// POST: User/AssignRole
		[HttpPost]
		public async Task<IActionResult> AssignRole(string userId, string roleName)
		{
			panelUser user = await panelUserManager.FindByIdAsync(userId);
			var roles = await panelUserManager.GetRolesAsync(user);


			foreach (var r in roles)
			{
				Console.WriteLine($"Kullanıcının rolü: '{r}'"); // Görsel olarak kontrol et
			}
			if (user == null)
			{
				return NotFound();
			}

			if (string.IsNullOrEmpty(roleName))
			{
				bool isSucc = await EmptyUserRole(userId);
				if (isSucc)
				{
					return RedirectToAction(nameof(AssignRole));
				}
				else
				{
					ModelState.AddModelError("", "Kullanıcının mevcut rolleri kaldırılamadı.");
					var users = await panelUserManager.Users.ToListAsync();
					var userRolesViewModelList = new List<UserRolesViewModel>();
					foreach (panelUser u in users)
					{
						var thisViewModel = new UserRolesViewModel();
						thisViewModel.UserId = u.Id.ToString();
						thisViewModel.Email = u.Email;
						thisViewModel.UserName = u.UserName;
						thisViewModel.Name = u.Name;
						thisViewModel.Surname = u.Surname;
						thisViewModel.Roles = await GetUserRoles(u);
						userRolesViewModelList.Add(thisViewModel);
					}
					ViewBag.Roles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
					return View("AssignRole", userRolesViewModelList);
				}
			}

			// Rolün var olup olmadığını kontrol et
			if (!await roleManager.RoleExistsAsync(roleName))
			{
				// Rol yoksa oluşturabilirsiniz veya hata mesajı dönebilirsiniz.
				// Şimdilik sadece bir log yazıp devam ediyorum.
				Console.WriteLine($"Rol bulunamadı: {roleName}");
				//ModelState.AddModelError("", $"Rol bulunamadı: {roleName}");
				//return RedirectToAction(nameof(AssignRole)); 
				// VEYA: await _roleManager.CreateAsync(new panelUserRole { Name = roleName });
			}
			user = await panelUserManager.FindByIdAsync(userId);
			var userRoles = await panelUserManager.GetRolesAsync(user);
			System.Diagnostics.Debug.WriteLine(string.Join(",", userRoles));
			// Kullanıcının mevcut rollerini kaldır
			var removeResult = await panelUserManager.RemoveFromRolesAsync(user, userRoles);
			if (!removeResult.Succeeded)
			{
				// Hata yönetimi
				ModelState.AddModelError("", "Kullanıcının mevcut rolleri kaldırılamadı.");
				// Hataları loglayabilir veya kullanıcıya gösterebilirsiniz.
				foreach (var error in removeResult.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				// Kullanıcı listesini ve rolleri tekrar yükleyip view'ı döndür
				var users = await panelUserManager.Users.ToListAsync();
				var userRolesViewModelList = new List<UserRolesViewModel>();
				foreach (panelUser u in users)
				{
					var thisViewModel = new UserRolesViewModel();
					thisViewModel.UserId = u.Id.ToString();
					thisViewModel.Email = u.Email;
					thisViewModel.UserName = u.UserName;
					thisViewModel.Name = u.Name;
					thisViewModel.Surname = u.Surname;
					thisViewModel.Roles = await GetUserRoles(u);
					userRolesViewModelList.Add(thisViewModel);
				}
				ViewBag.Roles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
				return View("AssignRole", userRolesViewModelList);
			}

			// Yeni rolü ekle
			var addResult = await panelUserManager.AddToRoleAsync(user, roleName);
			if (!addResult.Succeeded)
			{
				// Hata yönetimi
				ModelState.AddModelError("", "Yeni rol atanamadı.");
				foreach (var error in addResult.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				// Hata durumunda view'ı tekrar yükle
				var users = await panelUserManager.Users.ToListAsync();
				var userRolesViewModelList = new List<UserRolesViewModel>();
				foreach (panelUser u in users)
				{
					var thisViewModel = new UserRolesViewModel();
					thisViewModel.UserId = u.Id.ToString();
					thisViewModel.Email = u.Email;
					thisViewModel.UserName = u.UserName;
					thisViewModel.Name = u.Name;
					thisViewModel.Surname = u.Surname;
					thisViewModel.Roles = await GetUserRoles(u);
					userRolesViewModelList.Add(thisViewModel);
				}
				ViewBag.Roles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
				return View("AssignRole", userRolesViewModelList);
			}
			await panelUserManager.UpdateSecurityStampAsync(user);
			DynamicClaimsMiddleware.MarkUserForRefresh(user.Id.ToString());
			var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (currentUserId == userId)
			{
				// If updating current user, refresh their authentication immediately
				await signInManager.RefreshSignInAsync(user);
			}
			return RedirectToAction(nameof(AssignRole));
		}

		// GET: User/AddNewRole
		[Authorize(Policy = "CanViewPanelUserManagement")]
		public async Task<IActionResult> AddNewRole()
		{
			var roles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
			var rolesss = await roleManager.Roles.ToListAsync();
			var viewModel = new RoleViewModel();
			viewModel.Names = roles;
			return View(rolesss);

		}
		[Authorize(Policy = "CanEditPanelUserManagement")]
		// POST: User/AddNewRole
		[HttpPost]
		public async Task<IActionResult> AddNewRole(string roleName)
		{
			if (string.IsNullOrWhiteSpace(roleName))
			{
				ModelState.AddModelError("", "Rol adı boş olamaz.");
				return View((object)roleName); // View'a string model gönderiyoruz
			}

			var roleExist = await roleManager.RoleExistsAsync(roleName);
			if (roleExist)
			{
				ModelState.AddModelError("", "Bu rol zaten mevcut.");
				return View((object)roleName); // View'a string model gönderiyoruz
			}

			var result = await roleManager.CreateAsync(new panelUserRole { Name = roleName });

			if (result.Succeeded)
			{
				// Başarılı olursa kullanıcıları ve rolleri listeleyen sayfaya yönlendir
				return RedirectToAction(nameof(AddNewRole));
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
			return RedirectToAction(nameof(AddNewRole));
		}
		[Authorize(Policy = "CanEditPanelUserManagement")]
		public async Task<IActionResult> DeleteRole(string roleName)
		{
			if (string.IsNullOrWhiteSpace(roleName))
			{
				// Hata mesajı TempData veya ViewBag ile AddNewRole view'ına taşınabilir.
				TempData["ErrorMessage"] = "Silinecek rol adı belirtilmedi.";
				return RedirectToAction(nameof(AddNewRole));
			}

			var role = await roleManager.FindByNameAsync(roleName);
			if (role == null)
			{
				TempData["ErrorMessage"] = "Silinecek rol bulunamadı.";
				return RedirectToAction(nameof(AddNewRole));
			}

			// Role bağlı kullanıcı olup olmadığını kontrol et
			var usersInRole = await panelUserManager.GetUsersInRoleAsync(roleName);
			if (usersInRole.Any())
			{
				bool isEmpty = true;
				foreach (panelUser u in usersInRole)
				{
					isEmpty = await EmptyUserRole(u.Id.ToString());
				}
				if (!isEmpty)
				{
					TempData["ErrorMessage"] = $"'{roleName}' rolüne atanmış kullanıcılardan birinin rolü değiştirilemedi.";
					return RedirectToAction(nameof(AddNewRole));
				}
			}

			var result = await roleManager.DeleteAsync(role);
			if (!result.Succeeded)
			{
				// Hata mesajları TempData veya ViewBag ile AddNewRole view'ına taşınabilir.
				string errors = string.Join(", ", result.Errors.Select(e => e.Description));
				TempData["ErrorMessage"] = $"Rol silinirken hata oluştu: {errors}";
			}
			else
			{
				TempData["SuccessMessage"] = $"'{roleName}' rolü başarıyla silindi.";
			}
			var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			foreach (var u in usersInRole)
			{
				await panelUserManager.UpdateSecurityStampAsync(u);
				DynamicClaimsMiddleware.MarkUserForRefresh(u.Id.ToString());
				if (currentUserId == u.Id.ToString())
				{
					// Eğer silinen rol, güncellenen kullanıcının rolüyse, oturumunu yenile
					await signInManager.RefreshSignInAsync(u);
				}
			}
			return RedirectToAction("Index", "Home");
		}
		[HttpGet]
		[Authorize(Policy = "CanEditPanelUserManagement")]
		public async Task<IActionResult> EditRole(int id)
		{
			ClaimsViewModel claimsViewModel = new ClaimsViewModel();
			if (id == null)
			{
				// Hata mesajı TempData veya ViewBag ile AddNewRole view'ına taşınabilir.
				TempData["ErrorMessage"] = "Silinecek rol adı belirtilmedi.";
				return RedirectToAction(nameof(AddNewRole));
			}

			var role = await roleManager.FindByIdAsync(id.ToString());

			if (role == null)
			{
				TempData["ErrorMessage"] = "Rol bulunamadı.";
				return RedirectToAction(nameof(AddNewRole));
			}
			var claims = await roleManager.GetClaimsAsync(role);
			foreach (var i in claims)
			{
				claimsViewModel.ClaimType.Add(i.Type);
				claimsViewModel.ClaimValue.Add(i.Value);
				if (i.Value == "MobileUserManagement.Edit")
				{
					claimsViewModel.ClaimDisplayName.Add("Mobil Kullanıcı Yönetimi Düzenleme İzni");
				}
				else if (i.Value == "MobileUserManagement.View")
				{
					claimsViewModel.ClaimDisplayName.Add("Mobil Kullanıcı Yönetimi Görüntüleme İzni");
				}
				else if (i.Value == "PanelUserManagement.Edit")
				{
					claimsViewModel.ClaimDisplayName.Add("Panel Kullanıcı Yönetimi Düzenleme İzni");
				}
				else if (i.Value == "PanelUserManagement.View")
				{
					claimsViewModel.ClaimDisplayName.Add("Panel Kullanıcı Yönetimi Görüntüleme İzni");
				}
				else if (i.Value == "BasicModerator.Edit")
				{
					claimsViewModel.ClaimDisplayName.Add("Admin Paneli Düzenleme");
				}
				else if (i.Value == "BasicModerator.View")
				{
					claimsViewModel.ClaimDisplayName.Add("Admin Paneli Görüntüleme");
				}
			}
			claimsViewModel.UserRole = role;
			return View(claimsViewModel);
		}
		[Authorize(Policy = "CanEditPanelUserManagement")]
		[HttpPost]
		public async Task<IActionResult> EditRole(int id, string roleName, string per)
		{
			if (string.IsNullOrWhiteSpace(roleName))
			{
				// Hata mesajı TempData veya ViewBag ile AddNewRole view'ına taşınabilir.
				TempData["ErrorMessage"] = "Rol adı boş olamaz.";
				return RedirectToAction("Index", "Home");
			}

			var role = await roleManager.FindByIdAsync(id.ToString());
			if (role == null)
			{
				TempData["ErrorMessage"] = "Rol bulunamadı.";
				return RedirectToAction("Index", "Home");
			}
			role.Name = roleName;
			var isUSucced = await roleManager.UpdateAsync(role);
			if (!isUSucced.Succeeded)
			{
				TempData["ErrorMessage"] = "Rol güncellenemedi.";
				return RedirectToAction("Index", "Home");
			}
			var claims = await roleManager.GetClaimsAsync(role);
			List<string> claimValues = await GetClaimValues(claims.ToList());
			switch (per)
			{
				case "CanEditMobileUserManagement":
					if (!claimValues.Contains("MobileUserManagement.Edit"))
					{
						await roleManager.AddClaimAsync(role, new Claim("Permission", "MobileUserManagement.Edit"));
					}
					break;
				case "CanViewMobileUserManagement":
					if (!claimValues.Contains("MobileUserManagement.View"))
					{
						await roleManager.AddClaimAsync(role, new Claim("Permission", "MobileUserManagement.View"));
					}
					break;
				case "CanEditPanelUserManagement":
					if (!claimValues.Contains("PanelUserManagement.Edit"))
					{
						await roleManager.AddClaimAsync(role, new Claim("Permission", "PanelUserManagement.Edit"));
					}
					break;
				case "CanViewPanelUserManagement":
					if (!claimValues.Contains("PanelUserManagement.View"))
					{
						await roleManager.AddClaimAsync(role, new Claim("Permission", "PanelUserManagement.View"));
					}
					break;
				case "BasicModeratorEditPermission":
					if (!claimValues.Contains("BasicModerator.Edit"))
					{
						await roleManager.AddClaimAsync(role, new Claim("Permission", "BasicModerator.Edit"));
					}
					break;
				case "BasicModeratorViewPermission":
					if (!claimValues.Contains("BasicModerator.View"))
					{
						await roleManager.AddClaimAsync(role, new Claim("Permission", "BasicModerator.View"));
					}
					break;
			}
			var userInRole = await panelUserManager.GetUsersInRoleAsync(role.Name);
			foreach (var u in userInRole)
			{
				DynamicClaimsMiddleware.MarkUserForRefresh(u.Id.ToString());
			}
			RefreshRoleUsersAuthentication(roleName);
			return RedirectToAction("Index", "Home");
		}
		[HttpPost]
		public async Task<IActionResult> RemoveClaim(int id, string claimValue)
		{
			var role = await roleManager.FindByIdAsync(id.ToString());
			if (role == null)
			{
				TempData["ErrorMessage"] = "Rol bulunamadı.";
				return RedirectToAction("Index", "Home");
			}
			var claims = await roleManager.GetClaimsAsync(role);
			var claimToRemove = claims.FirstOrDefault(c => c.Value == claimValue);
			if (claimToRemove != null)
			{
				var result = await roleManager.RemoveClaimAsync(role, claimToRemove);
				if (result.Succeeded)
				{
					TempData["SuccessMessage"] = $"'{claimValue}' claim başarıyla kaldırıldı.";
					var userInRole = await panelUserManager.GetUsersInRoleAsync(role.Name);
					foreach(var u in userInRole)
					{
						DynamicClaimsMiddleware.MarkUserForRefresh(u.Id.ToString());
					}


					await RefreshRoleUsersAuthentication(role.Name);
				}
				else
				{
					string errors = string.Join(", ", result.Errors.Select(e => e.Description));
					TempData["ErrorMessage"] = $"Claim kaldırılırken hata oluştu: {errors}";
					return RedirectToAction("Index", "Home");
				}
			}
			else
			{
				TempData["ErrorMessage"] = $"'{claimValue}' claim bulunamadı.";
			}
			return RedirectToAction("Index", "Home");
		}
		[Authorize(Policy = "CanEditPanelUserManagement")]
		public async Task<IActionResult> DeletePanelUser(int id)
		{
			var user = await panelUserManager.FindByIdAsync(id.ToString());
			
			if (user != null)
			{
				if (user == await panelUserManager.GetUserAsync(User))
				{
					await signInManager.SignOutAsync();
					await panelUserManager.DeleteAsync(user);
					RedirectToAction("Index", "Login");
				}
				await panelUserManager.DeleteAsync(user);
			}
			return RedirectToAction("ChoseUserType", "User");
		}
		private async Task<List<string>> GetUserRoles(panelUser user)
		{
			return new List<string>(await panelUserManager.GetRolesAsync(user));
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
		private async Task<bool> EmptyUserRole(string userId)
		{
			var user = await panelUserManager.FindByIdAsync(userId);
			var userRoles = await panelUserManager.GetRolesAsync(user);
			var removeResult = await panelUserManager.RemoveFromRolesAsync(user, userRoles);
			if (!removeResult.Succeeded)
			{
				return false;
			}
			else { return true; }
		}
		private async Task<List<string>> GetClaimValues(List<Claim> claims)
		{
			List<string> claimValues = new List<string>();
			foreach (var claim in claims)
			{
				claimValues.Add(claim.Value);
			}

			return claimValues;
		}
		private async Task RefreshRoleUsersAuthentication(string roleName)
		{
			var usersInRole = await panelUserManager.GetUsersInRoleAsync(roleName);
			var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			foreach (var user in usersInRole)
			{
				await panelUserManager.UpdateSecurityStampAsync(user);
				if (currentUserId == user.Id.ToString())
				{
					// Eğer güncellenen rol, oturum açmış kullanıcının rolüyse, oturumunu yenile
					await signInManager.RefreshSignInAsync(user);
				}
			}
		}
	}
}
