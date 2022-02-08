using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Model
{
    public class Device
    {
        [Required]
        public String Name { get; set; }

        [Required]
        public String Platform { get; set; }

        public String OwnerEmail { get; set; }
    }
}
