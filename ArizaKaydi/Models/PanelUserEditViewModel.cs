using Microsoft.AspNetCore.Http; // IFormFile için

namespace ArizaKaydi.Models
{
	public class PanelUserEditViewModel
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public string UserName { get; set; }
		public string? ImageURL { get; set; }

		public string? passwordNow { get; set; }
		public string? Password { get; set; } 
		public string? ConfirmPassword { get; set; } 

		public IFormFile? ImageFile { get; set; } 
	}
}