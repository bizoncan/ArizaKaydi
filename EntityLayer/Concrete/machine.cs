using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class machine
    {
        [Key] public int id {  get; set; }
        public string name {  get; set; }
        public string desc {  get; set; }
        public int number {  get; set; }
        public string imgURL {  get; set; }    

    }
}
