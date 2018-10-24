﻿using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class Item
    {
        public ulong Id { get; set; }
        public ulong OwnerId { get; set; }
        public uint ItemId { get; set; }
        public ushort Location { get; set; }
        public uint BagIndex { get; set; }
        public uint StackCount { get; set; }
        public uint Charges { get; set; }
        public float Durability { get; set; }
        public uint ExpirationTimeLeft { get; set; }

        public Character Owner { get; set; }
    }
}
