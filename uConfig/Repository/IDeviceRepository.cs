using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uConfig.Model;

namespace uConfig.Repository
{
    interface IDeviceRepository
    {
        void RegisterDevice(Device device);

        bool IsDeviceAlreadyRegistered(Device device);

        List<Device> GetDevices(string ownerEmail);
    }
}
