using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerProfessionUpdateScalerValues)]
    public class ServerProfessionUpdateScalerValues : IWritable
    {
        public class ProfessionScaler : IWritable
        {
            int ScalerIndex { get; set; }
            int TradeskillId { get; set; }
            int Item2TypeId { get; set; }
            int Field_C_32bit { get; set; }
            float Field_10_f32 { get; set; }
            int Field_14_32bit { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ScalerIndex);
                writer.Write(TradeskillId);
                writer.Write(Item2TypeId);
                writer.Write(Field_C_32bit);
                writer.Write(Field_10_f32);
                writer.Write(Field_14_32bit);
            }
        }

        public List<ProfessionScaler> Scalers { get; set; } = new List<ProfessionScaler>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Scalers.Count);
            foreach (var scaler in Scalers)
            {
                scaler.Write(writer);
            }
        }
    }
}
