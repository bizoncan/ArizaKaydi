using ArizaKaydi.Models;
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
			PanelUserViewModel panelUserViewModel = new PanelUserViewModel();
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			panelUserViewModel.UserName = user.UserName;
			panelUserViewModel.ImageURL = user.ImageURL;
			var roles = await _userManager.GetRolesAsync(user);	
			panelUserViewModel.Role = roles.FirstOrDefault();
			return View(panelUserViewModel);
		}
	}
}
