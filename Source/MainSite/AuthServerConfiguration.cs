using NexusForever.Shared.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainSite
{
    public class AuthServerConfiguration
    {
        public NetworkConfig Network { get; set; }
        public DatabaseConfig Database { get; set; }
    }
}
