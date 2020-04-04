using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Entity
{
    public class TradeskillMaterial: ISaveCharacter
    {
        public TradeskillMaterialEntry Entry { get; }
        public ulong Owner { get; }
        public ushort MaterialId { get; }
        
        public ushort Amount
        {
            get => amount;
            set {
                amount = value;
                saveMask |= TradeskillMaterialSaveMask.Amount;
            }
        }
        private ushort amount;

        private TradeskillMaterialSaveMask saveMask;

        public TradeskillMaterial(CharacterTradeskillMaterialModel model)
        {
            Owner = model.Id;
            MaterialId = model.MaterialId;
            Amount = model.Amount;

            Entry = GameTableManager.Instance.TradeskillMaterial.GetEntry(MaterialId);

            saveMask = TradeskillMaterialSaveMask.None;
        }

        public TradeskillMaterial(ulong characterId, ushort materialId)
        {
            Owner = characterId;
            MaterialId = materialId;
            Amount = 0;

            Entry = GameTableManager.Instance.TradeskillMaterial.GetEntry(MaterialId);

            saveMask = TradeskillMaterialSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == TradeskillMaterialSaveMask.None)
                return;

            if ((saveMask & TradeskillMaterialSaveMask.Create) != 0)
            {
                var model = new CharacterTradeskillMaterialModel
                {
                    Id = Owner,
                    MaterialId = MaterialId,
                    Amount = amount
                };

                context.Add(model);
            }
            else
            {
                var model = new CharacterTradeskillMaterialModel
                {
                    Id = Owner,
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
