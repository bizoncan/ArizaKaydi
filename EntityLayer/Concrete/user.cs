﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class user
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }    
        public string Email { get; set; }   

    }
}
