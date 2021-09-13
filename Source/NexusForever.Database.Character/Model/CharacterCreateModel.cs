using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public class CharacterCreateModel
    {
        public byte Race { get; set; }
        public byte CreationStart { get; set; }
        public ushort Faction { get; set; }
        public uint WorldId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Rx { get; set; }
        public float Ry { get; set; }
        public float Rz { get; set; }
        public string Comment { get; set; }
    }
}
