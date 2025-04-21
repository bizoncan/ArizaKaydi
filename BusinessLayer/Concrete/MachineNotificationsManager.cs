using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
	public class MachineNotificationsManager : IMachineNotificationsService
	{

		IMachineNotificationsDal machineNotificationsDal;
		public MachineNotificationsManager(IMachineNotificationsDal machineNotificationsDal)
		{
			this.machineNotificationsDal = machineNotificationsDal;
		}

		public List<machineNotifications> GetAllWithMachineName()
		{
			return machineNotificationsDal.getNotificationsWtihMachineName();
		}

		public void TAdd(machineNotifications t)
		{
			machineNotificationsDal.Insert(t);
		}

		public machineNotifications TGetById(int id)
		{
			return machineNotificationsDal.GetById(id);
		}

		public List<machineNotifications> TGetList()
		{
			return machineNotificationsDal.GetList();
		}

		public void TRemove(machineNotifications t)
		{
			machineNotificationsDal.Delete(t);
		}

		public void TUpdate(machineNotifications t)
		{
			machineNotificationsDal.Update(t);
		}
	}
}
