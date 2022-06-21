using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uConfig.Model;

namespace uConfig.Repository
{
    interface IDeviceRepository
    {
        #region Device Management
        Task RegisterDevice(Device device);

        Task<bool> IsDeviceAlreadyRegistered(Device device);

        Task<List<Device>> GetDevices(int userId);

        Task<Device> GetDeviceById(Guid deviceId);

        Task UpdateDevice(Device device);

        Task DeleteDevice(Guid deviceId);

        #endregion

        #region Device Config Management

        Task CreateOrUpdateDeviceConfig(Guid deviceId, DeviceConfig deviceConfig);

        Task<DeviceConfig> GetDeviceConfig(Guid deviceId);

        #endregion
    }
}
