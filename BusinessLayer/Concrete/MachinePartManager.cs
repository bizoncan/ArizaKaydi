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
	public class MachinePartManager : IMachinePartService
	{
		IMachinePartDal _machinePartDal;
		public MachinePartManager(IMachinePartDal machinePartDal)
		{
			_machinePartDal = machinePartDal;
		}
		public void TAdd(machinePart t)
		{
			_machinePartDal.Insert(t);
		}

		public machinePart TGetById(int id)
		{
			return _machinePartDal.GetById(id);
		}

		public List<machinePart> TGetList()
		{
			return _machinePartDal.GetList();
		}

		public void TRemove(machinePart t)
		{
			_machinePartDal.Delete(t);
		}

		public void TUpdate(machinePart t)
		{
			_machinePartDal.Update(t);
		}
	}
}
