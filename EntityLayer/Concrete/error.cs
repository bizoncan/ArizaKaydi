using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class error
    {
        [Key]
        public int id { get; set; }
        public string errorType { get; set; }
        public int? machineId { get; set; }
        [JsonIgnore]
        public machine? machines { get; set; }
        public string errorDesc {  get; set; }
        public DateTime errorDate { get; set; }
        public DateTime errorEndDate { get; set; }
        public int? machinePartId { get; set; }
        [JsonIgnore]
        public machinePart? machinePartName { get; set; }
        public String? errorImage { get; set; }
        [JsonIgnore]
        public byte[]? errorImageBytes { get; set; }
        public String? errorImageType { get; set; }
        public int? userId { get; set; }
        [JsonIgnore]
        public user? userName { get; set; }
    }
}
