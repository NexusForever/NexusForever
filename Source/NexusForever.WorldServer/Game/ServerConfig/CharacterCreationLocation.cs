using System;
using System.Collections.Generic;
using System.Text;
using NexusForever.Database.World.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.ServerConfig.Static;

namespace NexusForever.WorldServer.Game.ServerConfig
{
    public class CharacterCreationLocation
    {
        public uint Id { get; set; }
        public CharacterCreation CharacterCreation { get; set; }
        public Race Race { get; set; }
        public Faction Faction { get; set; }
        public float StartingX { get; set; }
        public float StartingY { get; set; }
        public float StartingZ { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
        public ushort WorldId { get; set; }
        public ushort WorldZoneId { get; set; }

        public CharacterCreationLocation(ServerConfigCharacterCreationLocationModel model)
        {
            Id = model.Id;
            CharacterCreation = (CharacterCreation)model.CharacterCreationId;
            Race = (Race)model.RaceId;
            Faction = (Faction)model.FactionId;
            StartingX = model.StartingX;
            StartingY = model.StartingY;
            StartingZ = model.StartingZ;
            RotationX = model.RotationX;
            WorldId = model.WorldId;
            WorldZoneId = model.WorldZoneId;
        }
    }
}
