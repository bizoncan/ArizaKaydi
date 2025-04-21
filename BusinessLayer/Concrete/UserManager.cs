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
	public class UserManager : IUserService
	{
		IUserDal _userDal;

		public UserManager(IUserDal userDal)
		{
			_userDal = userDal;
		}

		public void TAdd(user t)
		{
			_userDal.Insert(t);
		}

		public user TGetById(int id)
		{
			return _userDal.GetById(id);
		}

		public List<user> TGetList()
		{
			return _userDal.GetList();
		}

		public void TRemove(user t)
		{
			 _userDal.Delete(t);
		}

		public void TUpdate(user t)
		{
			_userDal.Update(t);
		}
	}
}
