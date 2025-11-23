using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Fortunes
{
    // Not sure the level of use of this message
    // Triggers ClickEmpty sound if fortune game is not started or when value is 3
    [Message(GameMessageOpcode.ServerFortunesReset)]
    public class ServerFortunesReset : IWritable
    {
        public uint Unknown { get; set; } // 3 = click empty sound, only seen value 2 in sniffs

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown, 3u);
        }
    }
}
