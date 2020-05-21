using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.RBAC.Static;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Disable, "A collection of commands to manage entity disables.", "disable")]
    public class DisableCommandCategory : CommandCategory
    {
        [Command(Permission.DisableInfo, "Return information on the supplied disable and object id.", "info")]
        public void HandleDisableInfo(ICommandContext context,
            [Parameter("Disabled entity type.", ParameterFlags.None, typeof(EnumParameterConverter<DisableType>))]
            DisableType disableType,
            [Parameter("Object id for the disabled entity type.")]
            uint objectId)
        {
            string note = DisableManager.Instance.GetDisableNote(disableType, objectId);
            if (note == null)
            {
                context.SendMessage("That combination of disable type and object id isn't disabled!");
                return;
            }

            context.SendMessage($"Type: {disableType}, ObjectId: {objectId}, Note: {note}");
        }

        [Command(Permission.DisableReload, "Reload all entity disables from the database.", "reload")]
        public void HandleDisableReload(ICommandContext context)
        {
            DisableManager.Instance.Initialise();
            context.SendMessage("Reloaded disables from database.");
        }
    }
}
