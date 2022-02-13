using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uConfig.Model;

namespace uConfig.Repository
{
    public class InMemoryDeviceRepository : IDeviceRepository
    {
        private static readonly List<Device> RegisteredDevices = new List<Device>();
        private static readonly Dictionary<Guid, DeviceConfig> DeviceConfigs = new Dictionary<Guid, DeviceConfig>();

        #region Device Management
        public void RegisterDevice(Device device)
        {
            device.DeviceID = Guid.NewGuid();
            RegisteredDevices.Add(device);
        }

        public bool IsDeviceAlreadyRegistered(Device device)
        {
            int matchCount = RegisteredDevices.FindAll(registeredDevice => 
                registeredDevice.UserID == device.UserID && registeredDevice.Name.Equals(device.Name)).Count;

            return matchCount > 0;
        }

        public List<Device> GetDevices(int userId)
        {
            return RegisteredDevices.FindAll(device => device.UserID == userId);
        }

        public Device GetDeviceById(Guid deviceId)
        {
            return RegisteredDevices.Find(device => device.DeviceID.Equals(deviceId));
        }

        public void UpdateDevice(Device device)
        {
            Device deviceToUpdate = RegisteredDevices.Find(registeredDevice => registeredDevice.DeviceID == device.DeviceID);
            deviceToUpdate.Name = device.Name;
            deviceToUpdate.Platform = device.Platform;
        }

        #endregion Device Management

        #region Device Config Management

        public void CreateOrUpdateDeviceConfig(Guid deviceId, DeviceConfig deviceConfig)
        {
            DeviceConfigs.Remove(deviceId);
            DeviceConfigs.Add(deviceId, deviceConfig);
        }

        public DeviceConfig GetDeviceConfig(Guid deviceId)
        {
            if (DeviceConfigs.ContainsKey(deviceId))
            {
                return DeviceConfigs[deviceId];
            }

            return null;
        }

        #endregion
    }
}
