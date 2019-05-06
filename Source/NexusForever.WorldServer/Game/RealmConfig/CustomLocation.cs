using System.Numerics;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Game.RealmConfig
{
    public class CustomLocation
    {
        private readonly byte id;

        public uint CustomLocationId { get; }
        private Position Position { get; }
        private float Facing0 { get; }
        private float Facing1 { get; }
        private float Facing2 { get; }
        private float Facing3 { get; }
        private ushort WorldId { get; }
        private ushort WorldZoneId { get; }

        public CustomLocation(RealmConfigCustomLocation model)
        {
            id                  = model.Id;
            CustomLocationId    = model.CustomLocationId;
            Position            = new Position(new Vector3(model.Position0, model.Position1, model.Position2));
            Facing0             = model.Facing0;
            Facing1             = model.Facing1;
            Facing2             = model.Facing2;
            Facing3             = model.Facing3;
            WorldId             = model.WorldId;
            WorldZoneId         = model.WorldZoneId;
        }

        public WorldLocation2Entry ToWorldLocation()
        {
            return new WorldLocation2Entry
            {
                Id = CustomLocationId,
                Position0 = Position.Vector.X,
                Position1 = Position.Vector.Y, 
                Position2 = Position.Vector.Z, 
                Facing0 = Facing0,
                Facing1 = Facing1, 
                Facing2 = Facing2,
                Facing3 = Facing3,
                WorldId = WorldId,
                WorldZoneId = WorldZoneId
            };
        }
    }
}
