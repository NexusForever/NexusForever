using Microsoft.EntityFrameworkCore;
using NexusForever.WorldServer.Database.Character.Model;

namespace NexusForever.WorldServer.Database.Character
{
    public class CharacterContextExtended : CharacterContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CharacterCostume>()
                .Property(p => p.Index)
                .ValueGeneratedNever();

            modelBuilder.Entity<CharacterCostumeItem>()
                .Property(p => p.Slot)
                .ValueGeneratedNever();
        }
    }
}
