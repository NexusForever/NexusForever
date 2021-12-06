using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupPositionUpdate)]
    public class ServerGroupPositionUpdate : IWritable
    {
        public class UnknownStruct0
        {
            public TargetPlayerIdentity Identity { get; set; }
            public Position Position { get; set; }
            public uint Unknown0 { get; set; } //afaict this is never used in the client.
            public uint Flags { get; set; } = 0; // bInCombatPvp = 1, bIInCombatPve = 2, InCombat = 3
        }

        public ulong GroupId { get; set; }
        public uint WorldId { get; set; }
        public List<UnknownStruct0> Updates { get; set; } = new List<UnknownStruct0>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(WorldId, 15);

            writer.Write((uint)Updates.Count);
            Updates.ForEach(update => update.Identity.Write(writer));
            Updates.ForEach(update => update.Position.Write(writer));
            Updates.ForEach(update => writer.Write(update.Unknown0));
            Updates.ForEach(update => writer.Write(update.Flags));
        }
    }
}
