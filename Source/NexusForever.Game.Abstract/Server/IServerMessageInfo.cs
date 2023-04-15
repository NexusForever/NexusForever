namespace NexusForever.Game.Abstract.Server
{
    public interface IServerMessageInfo
    {
        byte Index { get; }
        List<string> Messages { get; }
    }
}