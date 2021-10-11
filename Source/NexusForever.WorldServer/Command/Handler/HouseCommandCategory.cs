using System.Text;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.House, "A collection of commands to modify housing residences.", "house")]
    [CommandTarget(typeof(Player))]
    public class HouseCommandCategory : CommandCategory
    {
        [Command(Permission.HouseDecor, "A collection of commands to modify decor in housing residences.", "decor")]
        public class HouseDecorCommandCategory : CommandCategory
        {
            [Command(Permission.HouseDecorAdd, "Add decor to housing residence crate optionally specifying quantity.", "add")]
            public void HandleHouseDecorAdd(ICommandContext context,
                [Parameter("Decor info id entry to add to the crate.")]
                uint decorInfoId,
                [Parameter("Quantity of decor to add to the crate.")]
                uint? quantity)
            {
                quantity ??= 1u;

                HousingDecorInfoEntry entry = GameTableManager.Instance.HousingDecorInfo.GetEntry(decorInfoId);
                if (entry == null)
                {
                    context.SendMessage($"Invalid decor info id {decorInfoId}!");
                    return;
                }

                context.GetTargetOrInvoker<Player>().ResidenceManager.DecorCreate(entry, quantity.Value);
            }

            [Command(Permission.HouseDecorLookup, "Returns a list of decor ids that match the supplied name.", "lookup")]
            public void HandleHouseDecorLookup(ICommandContext context,
                [Parameter("Name or partial name of the housing decor item to search for.")]
                string name)
            {
                var sw = new StringBuilder();
                sw.AppendLine("Decor Lookup Results:");

                TextTable tt = GameTableManager.Instance.GetTextTable(context.Language);
                foreach (HousingDecorInfoEntry decorEntry in
                    SearchManager.Instance.Search<HousingDecorInfoEntry>(name, context.Language, e => e.LocalizedTextIdName, true))
                {
                    string text = tt.GetEntry(decorEntry.LocalizedTextIdName);
                    sw.AppendLine($"({decorEntry.Id}) {text}");
                }

                context.SendMessage(sw.ToString());
            }
        }

        [Command(Permission.HouseTeleport, "Teleport to a residence, optionally specifying a character.", "teleport")]
        public void HandleHouseTeleport(ICommandContext context,
            [Parameter("", ParameterFlags.Optional)]
            string name)
        {
            Player target = context.GetTargetOrInvoker<Player>();
            if (!target.CanTeleport())
            {
                context.SendMessage("You have a pending teleport! Please wait to use this command.");
                return;
            }

            Residence residence = GlobalResidenceManager.Instance.GetResidenceByOwner(name ?? target.Name);
            if (residence == null)
            {
                if (name == null)
                    residence = GlobalResidenceManager.Instance.CreateResidence(target);
                else
                {
                    context.SendMessage("A residence for that character doesn't exist!");
                    return;
                }
            }

            ResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance(residence.PropertyInfoId);
            target.Rotation = entrance.Rotation.ToEulerDegrees();
            target.TeleportTo(entrance.Entry, entrance.Position, residence.Parent?.Id ?? residence.Id);
        }
    }
}
