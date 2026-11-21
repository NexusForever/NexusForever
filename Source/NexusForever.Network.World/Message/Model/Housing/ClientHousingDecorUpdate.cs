using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingDecorUpdate)]
    public class ClientHousingDecorUpdate : IReadable
    {
        public DecorUpdateOperation Operation { get; private set; }
        public List<DecorUpdate> DecorUpdates { get; private set; } = [];

        public void Read(GamePacketReader reader)
        {
            Operation = reader.ReadEnum<DecorUpdateOperation>(3u);

            uint count = reader.ReadUInt();
            for (uint i = 0u; i < count; i++)
            {
                var decorUpdate = new DecorUpdate();
                decorUpdate.Read(reader);
                DecorUpdates.Add(decorUpdate);
            }

            for (int i = 0; i < count; i++)
                DecorUpdates[i].UseServiceToken = reader.ReadBit();
        }
    }
}
