using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class machinePartError
    {
        [Key]
        public int id { get; set; }
        public int? machinePartId { get; set; }
        [JsonIgnore]
        public machinePart? machinePart { get; set; }
        public String title { get; set; }   
        public String description { get; set; }

    }
}
