using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
     
    public class GenericRepository<T> : IGenericDal<T> where T : class
    {
        public context _context;

        public GenericRepository(context context)
        {
            _context = context;
        }

        public void Delete(T t)
        {
    
            _context.Remove(t);
            _context.SaveChanges();
        }

        public T GetById(int id)
        {
            
            return _context.Set<T>().Find(id);
        }

        public List<T> GetList()
        {
          
            return _context.Set<T>().ToList();
        }

        public void Insert(T t)
        {

            _context.Add(t);
            _context.SaveChanges();
        }

        public void Update(T t)
        {
            _context.Update(t);
            _context.SaveChanges(); 
        }
        public error getNamesById(int id)
        {
            return _context.Errors.Include(e=> e.machines).Include(e=>e.machinePartName).Include(e=>e.userName).FirstOrDefault(e=>e.id==id);
            
        }
        public List<imageCollection> getImageCollectionByErrorId(int errorId)
        {
            return _context.ImageCollection.Where(e=>e.errorId== errorId).ToList();
        }
        public List<machineNotifications> getNotificationsWtihMachineName()
        {
			return _context.MachineNotifications.Include(e => e.machineName).ToList();
		}
        public work getWorkWithWorkOrderId(int id)
        {
            return _context.Works.Include(e => e.workOrder).Include(e => e.machinePart).Include(e => e.machine).Include(e => e.userI).FirstOrDefault(e => e.id == id);
		}
		public List<imageCollection> getImageCollectionByWorkId(int workId )
		{
			return _context.ImageCollection.Where(e => e.workId == workId).ToList();
		}
        public List<work> getWorkByWorkOrderList(int id)
		{
			return _context.Works.Where(e => e.workOrderId == id).ToList();
		}
        public List<error> getErrorsByDate(DateTime startDate, DateTime endDate) 
        {
            return _context.Errors
				.Where(e => e.errorDate >= startDate && e.errorDate <= endDate)
				.ToList();

		}
	}
}
