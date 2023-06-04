using NexusForever.Game.Abstract.Customisation;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Customisation
{
    public class CustomisationSelection : ICustomisationSelection
    {
        public uint Label { get; }
        public uint Value { get; }
        public uint Cost { get; }

        public CustomisationSelection(CharacterCustomizationSelectionEntry entry)
        {
            Label = entry.CharacterCustomizationLabelId;
            Value = entry.Value;
            // client also shifts this value, unsure why game table field is long
            Cost  = (uint)(entry.Cost >> 32);
        }
    }
}
