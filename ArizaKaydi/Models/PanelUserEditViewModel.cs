using Microsoft.AspNetCore.Http; // IFormFile için

namespace ArizaKaydi.Models
{
	public class PanelUserEditViewModel
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public string UserName { get; set; }
		public string? ImageURL { get; set; } // Mevcut resmi göstermek için

		public string? passwordNow { get; set; } // Mevcut şifre
		public string? Password { get; set; } // Yeni şifre
		public string? ConfirmPassword { get; set; } // Yeni şifre tekrar

		public IFormFile? ImageFile { get; set; } // Yüklenecek resim dosyası için
	}
}