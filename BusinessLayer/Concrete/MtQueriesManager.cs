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
	public class MtQueriesManager : IMtQueriesService
	{
		IMtQueriesDal _mtQueriesDal;
		public MtQueriesManager(IMtQueriesDal mtQueriesDal)
		{
			_mtQueriesDal = mtQueriesDal;
		}
		public void TAdd(mtQueries t)
		{
			_mtQueriesDal.Insert(t);
		}

		public mtQueries TGetById(int id)
		{
			return	_mtQueriesDal.GetById(id);
		}

		public List<mtQueries> TGetList()
		{
			return _mtQueriesDal.GetList();
		}

		public void TRemove(mtQueries t)
		{
			 _mtQueriesDal.Delete(t);
		}

		public void TUpdate(mtQueries t)
		{
			_mtQueriesDal.Update(t);
		}
	}
}
