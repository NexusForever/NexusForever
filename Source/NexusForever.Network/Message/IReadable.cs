namespace NexusForever.Network.Message
{
    public interface IReadable
    {
        void Read(GamePacketReader reader);
    }
}
