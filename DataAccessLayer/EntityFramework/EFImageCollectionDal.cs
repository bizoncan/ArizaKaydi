using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Repository;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFramework
{
	public class EFImageCollectionDal : GenericRepository<imageCollection>, IImageDalCollection
	{
		public EFImageCollectionDal(context context) : base(context)
		{
		}
	}
}
