using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Abstract.Customisation
{
    public interface ICustomisationLabel
    {
        uint Id { get; }
        string Name { get; }
        Faction Faction { get; }
    }
}