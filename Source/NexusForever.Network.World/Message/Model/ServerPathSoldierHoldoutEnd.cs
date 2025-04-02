using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSoldierHoldoutEnd)]
    public class ServerPathSoldierHoldoutEnd : IWritable
    {
        public enum PlayerPathSoldierResult
        {
            FailUnknown = 0x0,
            FailTimeOut = 0x1,
            FailDefenceDeath = 0x2,
            FailNoParticipants = 0x4,
            FailLeaveArea = 0x5,
            FailDeath = 0x6,
            FailLostResources = 0x3,
            FailParticipation = 0x7,
            ScriptCancel = 0x8,
            Success = 0x9,
        };

        public ushort PathSoldierEventId { get; set; }
        public PlayerPathSoldierResult Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSoldierEventId, 14);
            writer.Write(Reason);
        }
    }
}
