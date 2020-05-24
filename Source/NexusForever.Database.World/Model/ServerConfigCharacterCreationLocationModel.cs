using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.Database.World.Model
{
    public class ServerConfigCharacterCreationLocationModel
    {
        public uint Id { get; set; }
        public uint CharacterCreationId { get; set; }
        public uint RaceId { get; set; }
        public uint FactionId { get; set; }
        public float StartingX { get; set; }
        public float StartingY { get; set; }
        public float StartingZ { get; set; }
        public float RotationX { get; set; }
        public ushort WorldId { get; set; }
        public ushort WorldZoneId { get; set; }
        public ServerConfigModel ServerConfig { get; set; }
    }
}