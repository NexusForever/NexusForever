﻿using NexusForever.Game.Static;

namespace NexusForever.GameTable
{
    public class TextTableEntry
    {
        public uint Id { get; }
        public Language Language { get; }
        public string Text { get; }

        public TextTableEntry(Language language, uint id, string text)
        {
            Id       = id;
            Language = language;
            Text     = text;
        }
    }
}
