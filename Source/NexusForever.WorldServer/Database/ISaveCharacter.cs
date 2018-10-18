using NexusForever.WorldServer.Database.Character.Model;

namespace NexusForever.WorldServer.Database
{
    public interface ISaveCharacter
    {
        void Save(CharacterContext context);
    }
}
