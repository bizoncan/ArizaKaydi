using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
	public interface IImageCollectionService:IGenericService<imageCollection>
	{
		List<imageCollection> TGetImageCollectionByErrorId(int errorId);
		List<imageCollection> TGetImageCollectionByWorkId(int workId);
	}
}
