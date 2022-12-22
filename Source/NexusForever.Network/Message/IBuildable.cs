namespace NexusForever.Network.Message
{
    public interface IBuildable<out T>
    {
        T Build();
    }
}
