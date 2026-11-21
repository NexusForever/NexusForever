using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingPlaceDecor)]
    public class ClientHousingPlaceDecor : IReadable
    {
        public Identity ResidenceIdentity { get; private set; } = new();
        public ulong DecorId { get; private set; }
        public uint HousingDecorInfoId { get; private set; }
        public DecorOperation Operation { get; private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ResidenceIdentity.Read(reader);
            DecorId = reader.ReadULong();
            HousingDecorInfoId = reader.ReadUInt();
            Operation = reader.ReadEnum<DecorOperation>(3u);
            Position = reader.ReadVector3();
            Rotation = new Quaternion(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
        }
    }
}
