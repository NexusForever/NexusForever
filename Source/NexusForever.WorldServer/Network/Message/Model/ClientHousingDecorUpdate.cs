using System.Collections.Generic;
using System.Numerics;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingDecorUpdate)]
    public class ClientHousingDecorUpdate : IReadable
    {
        public DecorUpdateOperation Operation { get; private set; }
        public List<DecorUpdate> DecorUpdates { get; } = new List<DecorUpdate>();

        public class DecorUpdate : IReadable
        {
            public TargetPlayerIdentity Identity { get; } = new TargetPlayerIdentity();
            public ulong DecorId { get; private set; }
            public DecorType DecorType { get; private set; }
            public uint DecorData { get; private set; }
            public uint HookBagIndex { get; private set; }
            public uint HookIndex { get; private set; }
            public uint PlotIndex { get; private set; }
            public float Scale { get; private set; }
            public Vector3 Position { get; private set; }
            public Quaternion Rotation { get; private set; }
            public uint DecorInfoId { get; private set; }
            public uint ActivePropUnitId { get; private set; }
            public ulong ParentDecorId { get; private set; }
            public ushort ColourShiftId { get; private set; }

            public void Read(GamePacketReader reader)
            {
                Identity.Read(reader);
                DecorId          = reader.ReadULong();
                DecorType        = reader.ReadEnum<DecorType>(32u);
                DecorData        = reader.ReadUInt();
                HookBagIndex     = reader.ReadUInt();
                HookIndex        = reader.ReadUInt();
                PlotIndex        = reader.ReadUInt();
                Scale            = reader.ReadSingle();
                Position         = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Rotation         = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                DecorInfoId      = reader.ReadUInt();
                ActivePropUnitId = reader.ReadUInt();
                ParentDecorId    = reader.ReadULong();
                ColourShiftId    = reader.ReadUShort(14u);
            }
        }

        public void Read(GamePacketReader reader)
        {
            Operation = reader.ReadEnum<DecorUpdateOperation>(3u);

            uint count = reader.ReadUInt();
            for (uint i = 0u; i < count; i++)
            {
                var decor = new DecorUpdate();
                decor.Read(reader);
                DecorUpdates.Add(decor);
            }

            reader.ReadBit();
        }
    }
}
