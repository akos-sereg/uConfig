using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uConfig.Model;

namespace uConfig.DTOs
{
    public class StoreLogsRequest
    {
        public string Logs { get; set; }

        public DeviceDiagnostics Diagnostics { get; set; }
    }
}
