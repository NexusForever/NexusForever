namespace NexusForever.Network.Message
{
    public interface IWritable
    {
        void Write(GamePacketWriter writer);
    }
}
