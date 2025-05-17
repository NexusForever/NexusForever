using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Most commonly checked against worldLocation2->phases for publicEventObjective
    // Both properties used for group member visibility checks
    // Used for Prerequisite checks of type = 0x18 and 0x19
    [Message(GameMessageOpcode.ServerPhaseVisiblityWorldLocation)]
    public class ServerPhaseVisibilityWorldLocation : IWritable
    {
        public uint PhasesCanBeSeen { get; set; }
        public uint PhasesCanSee { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PhasesCanBeSeen);
            writer.Write(PhasesCanSee);
        }
    }
}
