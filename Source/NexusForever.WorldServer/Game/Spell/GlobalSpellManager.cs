namespace NexusForever.WorldServer.Game.Spell
{
    public static class GlobalSpellManager
    {
        /// <summary>
        /// Id to be assigned to the next spell cast.
        /// </summary>
        public static uint NextCastingId => nextCastingId++;

        private static uint nextCastingId;

        public static void Initialise()
        {
            nextCastingId = 1;
        }
    }
}
