using EntityLayer.Concrete;

namespace ArizaKaydi.Models
{
	public class ClaimsViewModel
	{
		public List<string> ClaimType { get; set; } = new List<string>();
		public List<string> ClaimValue { get; set; } = new List<string>();
		public List<string> ClaimDisplayName { get; set; } = new List<string>();
		public panelUserRole UserRole { get; set; }
		
		public ClaimsViewModel()
		{
			// Default constructor for model binding
		}
	}
}
