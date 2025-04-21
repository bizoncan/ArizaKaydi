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
	public class WorkManager : IWorkService
	{
		IWorkDal _workDal;

		public WorkManager(IWorkDal workDal)
		{
			_workDal = workDal;
		}

		public void TAdd(work t)
		{
			_workDal.Insert(t);
		}

		public work TGetById(int id)
		{
			return _workDal.GetById(id);
		}

		public List<work> TGetList()
		{
			return _workDal.GetList();
		}

	

		public List<work> TGetWorkByWorkOrderList(int id)
		{
			return _workDal.getWorkByWorkOrderList(id);
		}

		public work TGetWorkWithWorkOrder(int id)
		{
			return _workDal.getWorkWithWorkOrderId(id);
		}

		public void TRemove(work t)
		{
			_workDal.Delete(t);
		}

		public void TUpdate(work t)
		{
			_workDal.Update(t);
		}
	}
}
