using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Repository
{
    public interface IDeviceActivityRepository
    {
        Task RegisterDeviceCheckin(string deviceId, string endpoint);

        Task<DateTime?> GetLastSeen(string deviceId);

        Task<Dictionary<string, long>> GetLastSeenForDevices(int userId);

        Task AppendLogs(string deviceId, string messages);

        Task<List<string>> GetLogs(string deviceId);
    }
}
