using EntityLayer.Concrete;

namespace ArizaKaydi.Models
{
	public class MachinePartViewModel
	{
		public MachinePartViewModel()
		{
			machineParts = new List<MachinePartWorkCountViewModel>() ;
			machines = new List<machine>();
			
		}

		public List<MachinePartWorkCountViewModel> machineParts { get; set; }
		public List<machine> machines { get; set; }
		
		public int? selectedMachineId { get; set; }

	}
}
