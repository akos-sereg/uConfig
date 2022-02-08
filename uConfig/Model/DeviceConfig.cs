using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Model
{
    public class DeviceConfig
    {
        public String WifiSsidContext { get; set; }
        public List<DeviceConfigItem> Items { get; set; }

    }
}
