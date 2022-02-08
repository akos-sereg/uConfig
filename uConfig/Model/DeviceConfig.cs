using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Model
{
    public class DeviceConfig
    {
        public string WifiSsidContext { get; set; }

        [Required]
        public List<DeviceConfigItem> Items { get; set; }

    }
}
