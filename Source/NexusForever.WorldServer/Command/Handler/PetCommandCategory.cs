using NexusForever.Game.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Pet, "A collection of commands to managed pets for a character.", "pet")]
    [CommandTarget(typeof(Player))]
    public class PetCommandCategory : CommandCategory
    {
        [Command(Permission.PetUnlockFlair, "Unlock a pet flair for character.", "unlockflair")]
        public void HandlePetUnlockFlair(ICommandContext context,
            [Parameter("Pet flair entry id to unlock.")]
            ushort petFlairEntryId)
        {
            context.GetTargetOrInvoker<Player>().PetCustomisationManager.UnlockFlair(petFlairEntryId);
        }
    }
}
