using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingDecorUpdate)]
    public class ClientHousingDecorUpdate : IReadable
    {
        public DecorUpdateOperation Operation { get; private set; }
        public List<DecorInfo> DecorUpdates { get; } = new();

        public void Read(GamePacketReader reader)
        {
            Operation = reader.ReadEnum<DecorUpdateOperation>(3u);

            uint count = reader.ReadUInt();
            for (uint i = 0u; i < count; i++)
            {
                var decor = new DecorInfo();
                decor.Read(reader);
                DecorUpdates.Add(decor);
            }

            reader.ReadBit();
        }
    }
}
