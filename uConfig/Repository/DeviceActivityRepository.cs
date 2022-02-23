using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Repository
{
    public class DeviceActivityRepository : IDeviceActivityRepository
    {
        private static Dictionary<string, DateTime> _deviceLastSeen = new Dictionary<string, DateTime>();
        private static Dictionary<string, List<string>> _deviceLogs = new Dictionary<string, List<string>>();

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

        public void AppendLogs(string deviceId, string messages)
        {
            if (!_deviceLogs.ContainsKey(deviceId))
            {
                _deviceLogs.Add(deviceId, new List<string>());
            }

            string [] lines = messages.Split("\n");
            for (int i=0; i!=lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                {
                    continue;
                }

                _deviceLogs[deviceId].Add("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + lines[i]);
            }
        }

        public List<string> GetLogs(string deviceId)
        {
            if (_deviceLogs.ContainsKey(deviceId))
            {
                return _deviceLogs[deviceId];
            } else
            {
                return new List<string>();
            }
        }
    }
}
