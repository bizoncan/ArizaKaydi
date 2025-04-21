using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IGenericDal<T> where T : class
    {
        void Insert(T t);
        void Delete(T t);
        void Update(T t);
        List<T> GetList();
        T GetById(int id);
        error getNamesById(int id);
        List<imageCollection> getImageCollectionByErrorId(int errorId);
        List<machineNotifications> getNotificationsWtihMachineName();
        public work getWorkWithWorkOrderId(int id);
        public List<imageCollection> getImageCollectionByWorkId(int workId);
        public List<work> getWorkByWorkOrderList(int id);
        public List<error> getErrorsByDate(DateTime startDate, DateTime endDate);

	}
}
