namespace NexusForever.IO.Format.WildStar.M3
{
    public class MaterialDescription : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 296u;

        // index into the model textures, -1 if none
        public short ColourTextureSelector { get; set; }
        // index into the model textures, -1 if none
        public short NormalTextureSelector { get; set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            ColourTextureSelector = reader.ReadInt16();
            NormalTextureSelector = reader.ReadInt16();

            // remaining 292 bytes are unknown, they include further sub structures
        }
    }
}
