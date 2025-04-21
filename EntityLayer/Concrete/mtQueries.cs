using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
	public class mtQueries
	{
		[Key]
		public int id { get; set; }
		public string queryType { get; set; }
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		public int errorCount { get; set; }
		public double value { get; set; }
	}
}
