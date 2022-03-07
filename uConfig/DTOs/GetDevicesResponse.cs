using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uConfig.Model;

namespace uConfig.DTOs
{
    public class GetDevicesResponse
    {
        public List<Device> Devices { get; set; }
        public Dictionary<string, long> DeviceIdLastSeen { get; set; }

        public GetDevicesResponse(List<Device> devices, Dictionary<string, long> deviceIdToLastSeen)
        {
            this.Devices = devices;
            this.DeviceIdLastSeen = deviceIdToLastSeen;
        }
    }
}
