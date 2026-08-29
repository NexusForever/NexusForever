using NexusForever.Game.Static.PublicEvent;
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
        public uint ObjectiveData { get; set; } // Contents depends on objective type
                                                // For ActivateTargetGroupChecklist and TalkToChecklist, is a bitfield for objective completion
                                                // For ContestedArea and CapturePoint, is PublicEventTeamId indicating team owning the objective
        public uint DynamicMax { get; set; }
        public float Count { get; set; }
        public uint UnkState { get; set; }
        public PublicEventObjectiveDataType DataType { get; set; }
        public PublicEventTeam ControllingTeam { get; set; }
        public PublicEventTeam CapturingTeam { get; set; }
        public List<VirtualItem> VirtualItems { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Status, 32u);
            writer.Write(ObjectiveData);
            writer.Write(DynamicMax);
            writer.Write(Count);
            writer.Write(UnkState);
            writer.Write(DataType, 3);

            switch (DataType)
            {
                case PublicEventObjectiveDataType.Empty:
                    writer.Write((byte)0);
                    break;
                case PublicEventObjectiveDataType.CapturePointDefend:
                    writer.Write(ControllingTeam, 32u);
                    break;
                case PublicEventObjectiveDataType.CapturePoint:
                    writer.Write(CapturingTeam, 32u);
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
