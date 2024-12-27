using NexusForever.Game.Static.Event;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class PublicEventObjectiveStatus : IWritable
    {
        public class VirtualItem : IWritable
        {
            public uint ItemId { get; set; }
            public uint Count { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ItemId, 14);
                writer.Write(Count);
            }
        }

        public PublicEventStatus Status { get; set; }
        public uint Count { get; set; }
        public uint DynamicMax { get; set; }
        public float Percentage { get; set; }
        public uint SpellResourceIdx { get; set; }

        public PublicEventObjectiveDataType DataType { get; set; }
        public uint ControllingTeam { get; set; }
        public uint CapturingTeam { get; set; }
        public List<VirtualItem> VirtualItems { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Status, 32);
            writer.Write(Count);
            writer.Write(DynamicMax);
            writer.Write(Percentage);
            writer.Write(SpellResourceIdx);
            writer.Write(DataType, 3);

            switch (DataType)
            {
                case PublicEventObjectiveDataType.Empty:
                    writer.Write((byte)0);
                    break;
                case PublicEventObjectiveDataType.CapturePointDefend:
                    writer.Write(ControllingTeam);
                    break;
                case PublicEventObjectiveDataType.CapturePoint:
                    writer.Write(CapturingTeam);
                    break;
                case PublicEventObjectiveDataType.VirtualItemDepot:
                {
                    writer.Write(VirtualItems.Count);
                    foreach (VirtualItem item in VirtualItems)
                        item.Write(writer);
                    break;
                }
            }
        }
    }
}
