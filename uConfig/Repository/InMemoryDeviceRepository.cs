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

        public void RegisterDevice(Device device)
        {
            RegisteredDevices.Add(device);
        }

        public bool IsDeviceAlreadyRegistered(Device device)
        {
            int matchCount = RegisteredDevices.FindAll(registeredDevice => 
                registeredDevice.OwnerEmail.Equals(device.OwnerEmail) && registeredDevice.Name.Equals(device.Name)).Count;

            return matchCount > 0;
        }

        public List<Device> GetDevices(string ownerEmail)
        {
            return RegisteredDevices.FindAll(device => device.OwnerEmail.Equals(ownerEmail));
        }
    }
}
