using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerPathUnlockResult)]
    public class ServerPathUnlockResult : IWritable
    {
        public GenericError Result { get; set; }
        public PathUnlockedMask UnlockedPathMask { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 8u);
            writer.Write(UnlockedPathMask);
        }
    }
}
