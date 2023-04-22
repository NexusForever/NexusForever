using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Entity
{
    public class TradeskillMaterial : ITradeskillMaterial
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="ITradeskillMaterial"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum TradeskillMaterialSaveMask
        {
            None   = 0x0000,
            Create = 0x0001,
            Amount = 0x0002
        }

        public TradeskillMaterialEntry Entry { get; }
        public ulong Owner { get; }
        public ushort MaterialId { get; }

        public ushort Amount
        {
            get => amount;
            set
            {
                amount = value;
                saveMask |= TradeskillMaterialSaveMask.Amount;
            }
        }
        private ushort amount;

        private TradeskillMaterialSaveMask saveMask;

        public TradeskillMaterial(CharacterTradeskillMaterialModel model)
        {
            Owner      = model.Id;
            MaterialId = model.MaterialId;
            Amount     = model.Amount;

            Entry      = GameTableManager.Instance.TradeskillMaterial.GetEntry(MaterialId);

            saveMask   = TradeskillMaterialSaveMask.None;
        }

        public TradeskillMaterial(ulong characterId, ushort materialId)
        {
            Owner      = characterId;
            MaterialId = materialId;
            Amount     = 0;

            Entry      = GameTableManager.Instance.TradeskillMaterial.GetEntry(MaterialId);

            saveMask   = TradeskillMaterialSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == TradeskillMaterialSaveMask.None)
                return;

            if ((saveMask & TradeskillMaterialSaveMask.Create) != 0)
            {
                var model = new CharacterTradeskillMaterialModel
                {
                    Id         = Owner,
                    MaterialId = MaterialId,
                    Amount     = amount
                };

                context.Add(model);
            }
            else
            {
                var model = new CharacterTradeskillMaterialModel
                {
                    Id         = Owner,
                    MaterialId = MaterialId
                };

                EntityEntry<CharacterTradeskillMaterialModel> entity = context.Attach(model);
                if ((saveMask & TradeskillMaterialSaveMask.Amount) != 0)
                {
                    model.Amount = amount;
                    entity.Property(p => p.Amount).IsModified = true;
                }
            }

            saveMask = TradeskillMaterialSaveMask.None;
        }
    }
}
