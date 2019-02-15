using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class ServerMessage
    {
        public byte Index { get; set; }
        public byte Language { get; set; }
        public string Message { get; set; }
    }
}
