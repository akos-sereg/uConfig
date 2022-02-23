using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uConfig.Model;

namespace uConfig.DTOs
{
    public class CheckinResponse
    {
        public DeviceConfig DeviceConfig { get; set; }

        public long CurrentEpochInSeconds { get; set; }
    }
}
