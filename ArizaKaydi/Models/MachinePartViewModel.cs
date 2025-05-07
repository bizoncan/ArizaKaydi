using EntityLayer.Concrete;

namespace ArizaKaydi.Models
{
	public class MachinePartViewModel
	{
		public MachinePartViewModel()
		{
			machineParts = new List<machinePart>() ;
			machines = new List<machine>();
		}

		public List<machinePart> machineParts { get; set; }
		public List<machine> machines { get; set; }
		public int? selectedMachineId { get; set; }

	}
}
