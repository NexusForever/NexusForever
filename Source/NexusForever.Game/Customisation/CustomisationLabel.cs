using NexusForever.Game.Abstract.Customisation;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Customisation
{
    public class CustomisationLabel : ICustomisationLabel
    {
        public uint Id { get; }
        public string Name { get; }
        public Faction Faction { get; }

        public CustomisationLabel(CharacterCustomizationLabelEntry entry)
        {
            Id      = entry.Id;
            Name    = GameTableManager.Instance.TextEnglish.GetEntry(entry.LocalizedTextId);
            Faction = (Faction)entry.Faction2Id;
        }
    }
}
