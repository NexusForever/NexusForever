using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPlaneVisibilitySpellSlingerVoid)]
    public class ServerPlaneVisibilitySpellSlingerVoid : IWritable
    {
        public uint PlanesCanBeSeen { get; set; } // Not used by client
        public uint PlanesCanSee { get; set; }    // Only used for Prerequisite checks of type = 0xE8

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PlanesCanBeSeen);
            writer.Write(PlanesCanSee);
        }
    }
}
