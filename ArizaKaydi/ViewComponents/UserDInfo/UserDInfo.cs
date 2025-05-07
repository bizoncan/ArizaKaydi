using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArizaKaydi.ViewComponents.UserDInfo
{
	public class UserDInfo : ViewComponent 
	{

		UserManager<panelUser> userManager;
		public UserDInfo(UserManager<panelUser> userManager)
		{
			this.userManager = userManager;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var user = await userManager.FindByNameAsync(User.Identity.Name);
			return View(user);
		}
	}
}
