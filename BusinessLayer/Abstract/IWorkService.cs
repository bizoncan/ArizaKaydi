using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
	public interface IWorkService: IGenericService<work>
	{
		public work TGetWorkWithWorkOrder(int id);
		public List<work> TGetWorkByWorkOrderList(int id);
	}
}
