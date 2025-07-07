using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Pregame;

namespace NexusForever.Network.World.Message.Model
{
    // Response to ClientGetRealmTransferDestinations (0x3EE)
    // Triggers TransferDestinationRealmList lua event.
    [Message(GameMessageOpcode.ServerTransferDestinationRealmList)]
    public class ServerTransferDestinationRealmList : IWritable
    {
        public class RealmTransferInfo : RealmInfo
        {
            public bool IsFree { get; set; }

            public override void Write(GamePacketWriter writer)
            {
                base.Write(writer);
                writer.Write(IsFree);
            }
        }

        public List<RealmTransferInfo> Realms { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Realms.Count);
            Realms.ForEach(s => s.Write(writer));
        }
    }
}
