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

            modelBuilder.Entity<CharacterDatacube>()
                .Property(p => p.Type)
                .ValueGeneratedNever();

            modelBuilder.Entity<CharacterDatacube>()
                .Property(p => p.Datacube)
                .ValueGeneratedNever();

            modelBuilder.Entity<CharacterKeybinding>()
                .Property(p => p.InputActionId)
                .ValueGeneratedNever();

            modelBuilder.Entity<CharacterMailAttachment>()
                .Property(p => p.Index)
                .ValueGeneratedNever();

            modelBuilder.Entity<CharacterStats>()
                .Property(p => p.Stat)
                .ValueGeneratedNever();

            modelBuilder.Entity<ResidencePlot>()
                .Property(p => p.Index)
                .ValueGeneratedNever();
        }
    }
}
