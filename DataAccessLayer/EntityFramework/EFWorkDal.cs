﻿using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Repository;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFramework
{
	public class EFWorkDal:GenericRepository<work>, IWorkDal
	{
		public EFWorkDal(context context) : base(context)
		{
		}
		
	}
}
