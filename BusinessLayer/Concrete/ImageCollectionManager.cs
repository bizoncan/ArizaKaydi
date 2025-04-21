using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
	public class ImageCollectionManager : IImageCollectionService
	{
		IImageDalCollection _imageDalCollection;

		public ImageCollectionManager(IImageDalCollection ımageDalCollection)
		{
			_imageDalCollection = ımageDalCollection;
		}

		public void TAdd(imageCollection t)
		{
			_imageDalCollection.Insert(t);
		}

		public imageCollection TGetById(int id)
		{
			return _imageDalCollection.GetById(id);
		}

		public List<imageCollection> TGetImageCollectionByErrorId(int errorId)
		{
			return _imageDalCollection.getImageCollectionByErrorId(errorId);
		}

		public List<imageCollection> TGetImageCollectionByWorkId(int workId)
		{
			return _imageDalCollection.getImageCollectionByWorkId(workId);
		}

		public List<imageCollection> TGetList()
		{
			return _imageDalCollection.GetList();
		}

		public void TRemove(imageCollection t)
		{
			 _imageDalCollection.Delete(t);
		}

		public void TUpdate(imageCollection t)
		{
			 _imageDalCollection.Update(t);
		}
	}
}
