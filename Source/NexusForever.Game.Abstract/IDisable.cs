using NexusForever.Game.Static;

namespace NexusForever.Game.Abstract
{
    public interface IDisable
    {
        uint ObjectId { get; }
        DisableType Type { get; }
        string Note { get; }
    }
}