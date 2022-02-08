using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Model
{
    public class Device
    {
        public Guid DeviceID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Platform { get; set; }

        public string OwnerEmail { get; set; }
    }
}
