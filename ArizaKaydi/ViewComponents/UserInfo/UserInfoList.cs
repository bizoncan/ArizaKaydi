using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKaydi.ViewComponents.UserInfo
{
	public class UserInfoList:ViewComponent
	{
		UserManager<panelUser> _userManager;
		public UserInfoList(UserManager<panelUser> userManager)
		{
			_userManager = userManager;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			
			return View(await _userManager.FindByNameAsync(User.Identity.Name));
		}
	}
}
