using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entitlement
{
    public interface IEntitlementManager<T> : IEnumerable<T> where T : IEntitlement
    {
        /// <summary>
        /// Return <typeparamref name="T"/> for supplied <see cref="EntitlementType"/>.
        /// </summary>
        T GetEntitlement(EntitlementType type);

        /// <summary>
        /// Create or update account or character <see cref="EntitlementType"/> with supplied value.
        /// </summary>
        void UpdateEntitlement(EntitlementType type, int value);
    }
}