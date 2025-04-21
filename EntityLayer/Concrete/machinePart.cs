using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class machinePart
    {
        [Key]
        public int Id { get; set; }
        public int? machineId { get; set; }
        [JsonIgnore]
        public machine? machineName { get; set; }
        public String name { get; set; }
        public String desc {  get; set; }
        public int number {get; set; }
    }
}
