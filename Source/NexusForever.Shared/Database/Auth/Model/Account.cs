using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class Account
    {
        public uint Id { get; set; }
        public string Email { get; set; }
        public string S { get; set; }
        public string V { get; set; }
        public string GameToken { get; set; }
        public string SessionKey { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
