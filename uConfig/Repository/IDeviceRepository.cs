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

        bool IsDeviceAlreadyRegistered(Device device);

        List<Device> GetDevices(string ownerEmail);

        Device GetDeviceById(Guid deviceId);

        #endregion

        #region Device Config Management

        void CreateOrUpdateDeviceConfig(Guid deviceId, DeviceConfig deviceConfig);

        DeviceConfig GetDeviceConfig(Guid deviceId);

        #endregion
    }
}
