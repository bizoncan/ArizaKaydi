using System.ComponentModel.DataAnnotations;

namespace ArizaKaydi.Models
{
	public class PanelUserLoginViewModel
	{
		[Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
		public string UserName { get; set; }
		[Required(ErrorMessage = "Şifre zorunludur.")]
		public string Password { get; set; }
		
	}
}
