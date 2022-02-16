using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Repository
{
    public class DeviceActivityRepository : IDeviceActivityRepository
    {
        private static Dictionary<string, DateTime> _deviceLastSeen = new Dictionary<string, DateTime>();

        public void RegisterDeviceCheckin(string deviceId)
        {
            if (_deviceLastSeen.ContainsKey(deviceId))
            {
                _deviceLastSeen.Remove(deviceId);
            }

            _deviceLastSeen[deviceId] = DateTime.Now;
        }

        public DateTime? GetLastSeen(string deviceId)
        {
            DateTime? lastSeen = null;
            if (_deviceLastSeen.ContainsKey(deviceId))
            {
                lastSeen = _deviceLastSeen[deviceId];
            }

            return lastSeen;
        }
    }
}
