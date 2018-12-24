using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerPathUnlockResult, MessageDirection.Server)]
    public class ServerPathUnlockResult : IWritable
    {
        public byte Result { get; set; }
        public PathUnlockedMask UnlockedPathMask { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result);
            writer.Write(UnlockedPathMask);
        }
    }
}
