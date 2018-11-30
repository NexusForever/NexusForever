using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterBone
    {
        public ulong Id { get; set; }
        public byte BoneIndex { get; set; }
        public float Bone { get; set; }

        public Character IdNavigation { get; set; }
    }
}
