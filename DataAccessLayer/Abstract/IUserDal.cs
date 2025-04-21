using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
	public interface IUserDal : IGenericDal<user>
	{
		// Custom methods for User entity can be added here
		// For example, you might want to add a method to get users by role or status
		// List<User> GetUsersByRole(string role);
		// List<User> GetUsersByStatus(string status);
	}

}
