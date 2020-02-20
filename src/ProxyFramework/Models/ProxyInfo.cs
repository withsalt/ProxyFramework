using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyFramework.Models
{
    public class ProxyInfo
    {
        public List<string> IpAddress { get; set; }

        public uint HttpPort { get; set; }

        public uint Port { get; set; }

        public bool State { get; set; }
    }
}
