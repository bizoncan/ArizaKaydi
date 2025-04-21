using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class imageCollection
    {
        [Key]
        public int id {  get; set; }
        public int? errorId { get; set; }
        [JsonIgnore]
        public error? errorName {get; set; }
		public int? workId { get; set; }
		[JsonIgnore]
		public work? workName { get; set; }
		public byte[] imageDataByte { get; set; }
    }
}
