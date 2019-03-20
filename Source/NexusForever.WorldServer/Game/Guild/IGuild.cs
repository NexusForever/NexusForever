using NexusForever.Database.Character;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Guild
{
    /// <summary>
    /// This is used to pass a derived <see cref="GuildBase"/>into <see cref="CharacterDatabase"/> and be able to save GuildBaseModel and GuildSpecificModel
    /// </summary>
    public interface IGuild
    {
        void Save(CharacterContext context);
    }
}
