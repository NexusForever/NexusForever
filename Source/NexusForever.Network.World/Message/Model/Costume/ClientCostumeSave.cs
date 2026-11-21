using NexusForever.Game.Static.Costume;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Costume
{
    [Message(GameMessageOpcode.ClientCostumeSave)]
    public class ClientCostumeSave : IReadable
    {
        public class CostumeItem : IReadable
        {
            public uint Item2Id { get; private set; }
            public uint[] DyeColorRampIds { get; } = new uint[3];

            public void Read(GamePacketReader reader)
            {
                Item2Id = reader.ReadUInt(18u);
                for (int i = 0; i < 3; i++)
                    DyeColorRampIds[i] = reader.ReadUInt();
            }
        }

        public int Index { get; private set; }
        public CostumeType Type { get; private set; }
        public ulong MannequinDecorId { get; private set; }
        public List<CostumeItem> Items { get; } = []; // in order of CostumeItemSlot enum
        public uint VisibilityMask { get; private set; }
        public bool UserServiceToken { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Index            = reader.ReadInt();
            Type             = reader.ReadEnum<CostumeType>(2u);
            MannequinDecorId = reader.ReadULong();

            for (int i = 0; i < Costume.MaxCostumeItems; i++)
            {
                var part = new CostumeItem();
                part.Read(reader);
                Items.Add(part);
            }

            VisibilityMask  = reader.ReadUInt();
            UserServiceToken = reader.ReadBit();
        }
    }
}
