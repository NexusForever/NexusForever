using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entitlement
{
    public interface IEntitlement
    {
        uint Amount { get; set; }
        EntitlementEntry Entry { get; }
        EntitlementType Type { get; }
    }
}