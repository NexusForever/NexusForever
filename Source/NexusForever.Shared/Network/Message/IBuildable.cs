namespace NexusForever.Shared.Network.Message
{
    public interface IBuildable<out T> where T : IWritable
    {
        T Build();
    }
}
