using NexusForever.API.Character.Character;

namespace NexusForever.API.Character
{
    public static class GetCharacterEndpoint
    {
        public static void MapGetCharacterEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/character/{realmId:int}/{characterId:long}", async (CharacterManager characterManager, ushort realmId, ulong characterId) =>
            {
                Model.Character.Character character = await characterManager.GetCharacterAsync(realmId, characterId);
                if (character == null)
                    return Results.NotFound();

                return Results.Ok(character);
            });

            app.MapGet("/character/{realmName}/{characterName}", async (CharacterManager characterManager, string realmName, string characterName) =>
            {
                Model.Character.Character character = await characterManager.GetCharacterAsync(realmName, characterName);
                if (character == null)
                    return Results.NotFound();

                return Results.Ok(character);
            });
        }
    }
}
