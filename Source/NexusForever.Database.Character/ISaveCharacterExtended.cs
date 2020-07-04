namespace NexusForever.Database.Character
{
    public interface ISaveCharacterExtended
    {
        void Save(ulong characterId, CharacterContext context);
    }
}
