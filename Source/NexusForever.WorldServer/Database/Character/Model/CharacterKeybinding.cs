using System;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterKeybinding
    {
        public ulong Id { get; set; }
        public ushort InputActionId { get; set; }
        public uint DeviceEnum00 { get; set; }
        public uint DeviceEnum01 { get; set; }
        public uint DeviceEnum02 { get; set; }
        public uint Code00 { get; set; }
        public uint Code01 { get; set; }
        public uint Code02 { get; set; }
        public uint MetaKeys00 { get; set; }
        public uint MetaKeys01 { get; set; }
        public uint MetaKeys02 { get; set; }
        public uint EventTypeEnum00 { get; set; }
        public uint EventTypeEnum01 { get; set; }
        public uint EventTypeEnum02 { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}
