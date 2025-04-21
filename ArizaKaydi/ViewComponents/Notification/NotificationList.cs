using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ArizaKaydi.ViewComponents.Notification
{
	public class NotificationList : ViewComponent
	{
		MachineNotificationsManager machineNotificationsManager;
		public NotificationList(context context)
		{		
			machineNotificationsManager = new MachineNotificationsManager(new EFMachineNotificationsDal(context));
		}
		public IViewComponentResult Invoke()
		{
			var values = machineNotificationsManager.GetAllWithMachineName();
			return View(values);
		}
	}
}
