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

        static InMemoryDeviceRepository() {
            RegisteredDevices.Add(new Device()
            {
                DeviceID = Guid.Parse("00f2a002-1ae7-4727-8093-4ab01b0ab3ee"),
                Name = "Test",
                Platform = "esp32",
                UserID = 1234
            });

            DeviceConfigs.Add(
                Guid.Parse("00f2a002-1ae7-4727-8093-4ab01b0ab3ee"), 
                new DeviceConfig() { Items = new List<DeviceConfigItem>() { } });
        }

        #region Device Management
        public void RegisterDevice(Device device)
        {
            device.DeviceID = Guid.NewGuid();
            RegisteredDevices.Add(device);
        }

        public async Task<bool> IsDeviceAlreadyRegistered(Device device)
        {
            int matchCount = RegisteredDevices.FindAll(registeredDevice => 
                registeredDevice.UserID == device.UserID && registeredDevice.Name.Equals(device.Name)).Count;

            return matchCount > 0;
        }

        public async Task<List<Device>> GetDevices(int userId)
        {
            return RegisteredDevices.FindAll(device => device.UserID == userId);
        }

        public async Task<Device> GetDeviceById(Guid deviceId)
        {
            return RegisteredDevices.Find(device => device.DeviceID.Equals(deviceId));
        }

        public void UpdateDevice(Device device)
        {
            Device deviceToUpdate = RegisteredDevices.Find(registeredDevice => registeredDevice.DeviceID == device.DeviceID);
            deviceToUpdate.Name = device.Name;
            deviceToUpdate.Platform = device.Platform;
        }

        public void DeleteDevice(Guid deviceId)
        {
            RegisteredDevices.Remove(RegisteredDevices.Find(device => device.DeviceID == deviceId));
        }

        #endregion Device Management

        #region Device Config Management

        public async Task CreateOrUpdateDeviceConfig(Guid deviceId, DeviceConfig deviceConfig)
        {
            DeviceConfigs.Remove(deviceId);
            DeviceConfigs.Add(deviceId, deviceConfig);
        }

        public async Task<DeviceConfig> GetDeviceConfig(Guid deviceId)
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
