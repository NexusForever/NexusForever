using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Most commonly checked against worldLocation2->phases for publicEventObjective
    // Both properties used for group member visibility checks
    // Used for Prerequisite checks of type = 0x18 and 0x19
    [Message(GameMessageOpcode.ServerPhaseVisibilityWorldLocation)]
    public class ServerPhaseVisibilityWorldLocation : IWritable
    {
        public uint PhasesThatPerceiveMe { get; set; }
        public uint PhasesIPerceive { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PhasesThatPerceiveMe);
            writer.Write(PhasesIPerceive);
        }
    }
}
