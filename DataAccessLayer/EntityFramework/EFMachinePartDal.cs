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
	public class EFMachinePartDal:GenericRepository<machinePart>, IMachinePartDal
	{
		public EFMachinePartDal(context context) : base(context)
		{
		}
	}
	
}
