using NexusForever.Database.Character;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ITradeskillMaterial : IDatabaseCharacter
    {
        TradeskillMaterialEntry Entry { get; }
        ulong Owner { get; }
        ushort MaterialId { get; }
        ushort Amount { get; set; }
    }
}