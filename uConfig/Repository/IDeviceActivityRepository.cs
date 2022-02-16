﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Repository
{
    public interface IDeviceActivityRepository
    {
        void RegisterDeviceCheckin(string deviceId);

        DateTime? GetLastSeen(string deviceId);
    }
}