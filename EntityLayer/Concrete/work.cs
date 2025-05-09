using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
	public class work
	{
		[Key]
		public int id { get; set; }
		public int workOrderId { get; set; }
		[JsonIgnore] 
		public workOrder? workOrder { get; set; }
		[Required(ErrorMessage = "Lütfen bir başlık giriniz.")]
		[StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
		public String title { get; set; }
		[Required(ErrorMessage = "Lütfen bir açıklama giriniz.")]
		[StringLength(2000, ErrorMessage = "Başlık en fazla 2000 karakter olabilir.")]
		public String desc { get; set; }
		public bool isClosed { get; set; }
		public bool isOpened { get; set; }
		public DateTime? workOrderStartDate { get; set; }
		public DateTime? workOrderEndDate { get; set; }
		public int? machineId { get; set; }
		[JsonIgnore]
		public machine? machine { get; set; }
		public int? machinePartId { get; set; }
		[JsonIgnore]
		public machinePart? machinePart { get; set; }
		public int? userId { get; set; }
		[JsonIgnore]
		public user? userI { get; set; }
		public bool isPastWork { get; set; }
	}
}
