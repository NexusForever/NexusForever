using NexusForever.Game.Static.PlayerPath;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.PlayerPath
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
