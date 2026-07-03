using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.IpServices
{
    public class IpServiceSettings
    {
        public bool EnableIpAnonymization { get; set; } = true;
        public bool EnableIpHashing { get; set; } = true;
        public bool StoreRawIp { get; set; } = false;
    }
}
