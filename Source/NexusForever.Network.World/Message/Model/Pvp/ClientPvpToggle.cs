using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pvp
{
    // If pvp cooldown time remains, client can only set the Value to 1.
    // If there is no pvp cooldown remaining, client can only set the Value 0.  
    // Some logic in the handler necessary to figure out how to latch/unlatch the pvp
    // state with the progress of the cooldown timer.
    [Message(GameMessageOpcode.ClientPvpToggleFlags)]
    public class ClientPvpToggleFlags : IReadable
    {
        public bool Value { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Value = reader.ReadBit();
        }
    }
}
