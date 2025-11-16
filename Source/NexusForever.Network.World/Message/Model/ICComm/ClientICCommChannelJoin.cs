using NexusForever.Game.Static.ICComm;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.ICComm
{
    [Message(GameMessageOpcode.ClientICCommChannelJoin)]
    public class ClientICCommChannelJoin : IReadable
    {
        public ICCommChannelType Type { get; private set; }
        public ulong GuildId { get; private set; } // only sent if Type is Guild
        public string Name { get; private set; } 

        public void Read(GamePacketReader reader)
        {
            Type = reader.ReadEnum<ICCommChannelType>(3u);
            GuildId = reader.ReadULong();
            Name = reader.ReadWideString();
        }
    }
}
