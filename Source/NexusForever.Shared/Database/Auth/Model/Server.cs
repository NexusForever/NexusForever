using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class Server
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public ushort Port { get; set; }
        public byte Type { get; set; }
    }
}
