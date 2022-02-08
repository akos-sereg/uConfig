using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Model
{
    public class DeviceConfigItem
    {
        public String Key { get; set; }

        public String Value { get; set; }

        public List<DeviceConfigItemOverride> WifiZoneOverrides { get; set; }
    }
}
