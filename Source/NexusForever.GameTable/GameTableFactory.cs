namespace NexusForever.GameTable
{
    public static class GameTableFactory
    {
        public static object LoadGameTable(Type type, string filePath)
        {
            Type table = typeof(GameTable<>).MakeGenericType(type);
            return Activator.CreateInstance(table, filePath);
        }

        public static TextTable LoadTextTable(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            return new TextTable(filePath);
            //return FileCache.LoadWithCache(filePath, file => new TextTable(file));
        }
    }
}