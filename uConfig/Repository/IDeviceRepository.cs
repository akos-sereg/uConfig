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
        void RegisterDevice(Device device);

        Task<bool> IsDeviceAlreadyRegistered(Device device);

        Task<List<Device>> GetDevices(int userId);

        Task<Device> GetDeviceById(Guid deviceId);

        void UpdateDevice(Device device);

        void DeleteDevice(Guid deviceId);

        #endregion

        #region Device Config Management

        public Task CreateOrUpdateDeviceConfig(Guid deviceId, DeviceConfig deviceConfig);

        Task<DeviceConfig> GetDeviceConfig(Guid deviceId);

        #endregion
    }
}
