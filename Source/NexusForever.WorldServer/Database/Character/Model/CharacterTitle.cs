using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterTitle
    {
        public ulong Id { get; set; }
        public ushort Title { get; set; }
        public uint TimeRemaining { get; set; }
        public byte Revoked { get; set; }

        public Character IdNavigation { get; set; }
    }
}
