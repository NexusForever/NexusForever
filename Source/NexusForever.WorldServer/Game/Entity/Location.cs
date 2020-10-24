using NexusForever.Shared.GameTable.Model;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Location
    {
        public WorldEntry World { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }

        public Location(WorldEntry world, Vector3 position, Vector3 rotation)
        {
            World = world;
            Position = position;
            Rotation = rotation;
        }
    }
}
