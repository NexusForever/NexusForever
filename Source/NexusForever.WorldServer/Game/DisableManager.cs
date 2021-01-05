using System.Collections.Immutable;
using NexusForever.Database.World.Model;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Game
{
    public sealed class DisableManager : AbstractManager<DisableManager>
    {
        private static ulong Hash(DisableType type, uint objectId)
        {
            // hash where type takes up the top 32 bits and objectId takes up the lower 32 bits
            return ((ulong)type << 32) | objectId;
        }

        private ImmutableDictionary<ulong, Disable> disables;

        private DisableManager()
        {
        }

        public override DisableManager Initialise()
        {
            var builder = ImmutableDictionary.CreateBuilder<ulong, Disable>();
            foreach (DisableModel model in DatabaseManager.Instance.WorldDatabase.GetDisables())
            {
                DisableType type = (DisableType)model.Type;
                builder.Add(Hash(type, model.ObjectId), new Disable(type, model.ObjectId, model.Note));
            }

            disables = builder.ToImmutable();
            return Instance;
        }

        /// <summary>
        /// Returns if <see cref="DisableType"/> and objectId are disabled.
        /// </summary>
        public bool IsDisabled(DisableType type, uint objectId)
        {
            ulong hash = Hash(type, objectId);
            return disables.ContainsKey(hash);
        }

        /// <summary>
        /// Return the disable reason for <see cref="DisableType"/> and objectId.
        /// </summary>
        public string GetDisableNote(DisableType type, uint objectId)
        {
            ulong hash = Hash(type, objectId);
            return disables.TryGetValue(hash, out Disable disable) ? disable.Note : null;
        }
    }
}
