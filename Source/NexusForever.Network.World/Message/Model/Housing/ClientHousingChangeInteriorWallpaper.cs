using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingChangeInteriorWallpaper)]
    public class ClientHousingChangeInteriorWallpaper : IReadable
    {
        public uint[] LayerUpdated { get; private set; } = new uint[6];
        public DecorInfo[] DecorUpdates { get; private set; } = new DecorInfo[6];

        public void Read(GamePacketReader reader)
        {
            for(int i = 0; i < LayerUpdated.Length; i++)
                LayerUpdated[i] = reader.ReadUInt();

            for (int i = 0; i < DecorUpdates.Length; i++)
                DecorUpdates[i].Read(reader);
        }
    }
}
