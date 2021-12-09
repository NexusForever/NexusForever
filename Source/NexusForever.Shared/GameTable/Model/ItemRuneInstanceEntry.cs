namespace NexusForever.Shared.GameTable.Model
{
    public class ItemRuneInstanceEntry
    {
        public uint Id;
        public uint DefinedSocketCount;
        [GameTableFieldArray(8u)]
        public uint[] DefinedSocketTypes;
        public uint ItemSetId;
        public uint ItemSetPower;
        public uint SocketCountMax;
    }
}
