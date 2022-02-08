using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Model
{
    public class DeviceConfigItem
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        public List<DeviceConfigItemOverride> WifiZoneOverrides { get; set; }
    }
}
