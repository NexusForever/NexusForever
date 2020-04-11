using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicActor)]
    public class ServerCinematicActor : IWritable
    {
        public uint Delay { get; set; }
        public ushort Flags { get; set; }
        public ushort Unknown0 { get; set; }
        public uint SpawnHandle { get; set; }
        public uint CreatureType { get; set; }
        public uint MovementMode { get; set; }
        public Position InitialPosition { get; set; }
        public ulong ActivePropId { get; set; }
        public uint SocketId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(Flags);
            writer.Write(Unknown0);
            writer.Write(SpawnHandle);
            writer.Write(CreatureType);
            writer.Write(MovementMode);
            InitialPosition.Write(writer);
            writer.Write(ActivePropId);
            writer.Write(SocketId);
        }
    }
}
