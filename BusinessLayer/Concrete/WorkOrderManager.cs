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
	public class WorkOrderManager : IWorkOrderService
	{
		IWorkOrderDal _workOrderDal;

		public WorkOrderManager(IWorkOrderDal workOrderDal)
		{
			_workOrderDal = workOrderDal;
		}

		public void TAdd(workOrder t)
		{
			_workOrderDal.Insert(t);
		}

		public workOrder TGetById(int id)
		{
			return _workOrderDal.GetById(id);	
		}

		public List<workOrder> TGetList()
		{
			return _workOrderDal.GetList();
		}

		public void TRemove(workOrder t)
		{
			_workOrderDal.Delete(t);
		}

		public void TUpdate(workOrder t)
		{
			_workOrderDal.Update(t);
		}
	}
}
