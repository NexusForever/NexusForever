using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicActorAdd)]
    public class ServerCinematicActorAdd : IWritable
    {
        public uint Delay { get; set; }
        public EntityCreateFlag Flags { get; set; }
        public ushort TextureLevelOfDetailBias { get; set; } // outside chance this is a signed value but sniffs do not show it used that way
        public uint UnitId { get; set; }
        public uint Creature2Id { get; set; }
        public ModeType MovementMode { get; set; }
        public Position Position { get; set; }
        public ulong ActivePropId { get; set; }
        public uint WorldSocketId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(Flags, 16u);
            writer.Write(TextureLevelOfDetailBias);
            writer.Write(UnitId);
            writer.Write(Creature2Id);
            writer.Write(MovementMode);
            Position.Write(writer);
            writer.Write(ActivePropId);
            writer.Write(WorldSocketId);
        }
    }
}
