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
    public class MachineManager : IMachineService
    {
        IMachineDal _machineDal;
        public MachineManager(IMachineDal _machineDal){
        this._machineDal = _machineDal;
        }
   
        public void TAdd(machine t)
        {
            _machineDal.Insert(t);
        }

        public machine TGetById(int id)
        {
            return _machineDal.GetById(id);
        }

        public List<machine> TGetList()
        {
            return _machineDal.GetList();
        }

        public void TRemove(machine t)
        {
            _machineDal.Delete(t);
        }

        public void TUpdate(machine t)
        {
            _machineDal.Update(t);
        }
    }
}
