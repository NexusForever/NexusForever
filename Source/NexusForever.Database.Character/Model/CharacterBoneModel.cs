namespace NexusForever.Database.Character.Model
{
    public class CharacterBoneModel
    {
        public ulong Id { get; set; }
        public byte BoneIndex { get; set; }
        public float Bone { get; set; }

        public CharacterModel Character { get; set; }
    }
}
