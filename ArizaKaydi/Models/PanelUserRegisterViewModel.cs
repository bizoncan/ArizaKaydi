using System.ComponentModel.DataAnnotations;

namespace ArizaKaydi.Models
{
	public class PanelUserRegisterViewModel
	{
		[Required(ErrorMessage = "İsim zorunludur.")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Soyisim zorunludur.")]
		public string Surname { get; set; }
		[Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
		public string UserName { get; set; }
		[Required(ErrorMessage = "Şifre zorunludur.")]
		public string Password { get; set; }
		[Required(ErrorMessage = "Şifre tekrar zorunludur.")]
		[Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
		public string  ConfirmPassword { get; set; }
		[Required(ErrorMessage = "Mail zorunludur.")]
		public string Email { get; set; }
	}
}
