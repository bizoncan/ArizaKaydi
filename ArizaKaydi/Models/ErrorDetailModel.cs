using EntityLayer.Concrete;
using System.Text.Json.Serialization;

namespace ArizaKaydi.Models
{
    public class ErrorDetailModel
    {
		public error errorInfo {  get; set; }
		public List<string> collection { get; set; }
	}
}
