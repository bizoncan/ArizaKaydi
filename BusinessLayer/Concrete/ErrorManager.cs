using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Abstract;


namespace BusinessLayer.Concrete
{
    
    public class ErrorManager : IErrorService
    {
        IErrorDal _errorDal;
        public ErrorManager(IErrorDal errorDal)
        {
            this._errorDal = errorDal;
        }

        public error TGetMachineInfos(int id)
        {
            var aa= _errorDal.getNamesById(id);
			return _errorDal.getNamesById(id);
        }

        public void TAdd(error t)
        {
            _errorDal.Insert(t);
        }

        public error TGetById(int id)
        {
            return _errorDal.GetById(id);
        }

        public List<error> TGetList()
        {
            return _errorDal.GetList();
        }

        public void TRemove(error t)
        {
            _errorDal.Delete(t);
        }

        public void TUpdate(error t)
        {
            _errorDal.Update(t);
        }

		public List<error> TGetErrorsByDate(DateTime starttDate, DateTime endDate)
		{
			return _errorDal.getErrorsByDate(starttDate, endDate);
		}
	}
}
