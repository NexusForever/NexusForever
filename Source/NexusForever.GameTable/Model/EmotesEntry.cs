namespace NexusForever.GameTable.Model
{
    public class EmotesEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdNoArgToAll { get; set; }
        public uint LocalizedTextIdNoArgToSelf { get; set; }
        public uint NoArgAnim { get; set; }
        public uint LocalizedTextIdArgToAll { get; set; }
        public uint LocalizedTextIdArgToArg { get; set; }
        public uint LocalizedTextIdArgToSelf { get; set; }
        public uint ArgAnim { get; set; }
        public uint LocalizedTextIdSelfToAll { get; set; }
        public uint LocalizedTextIdSelfToSelf { get; set; }
        public uint SelfAnim { get; set; }
        public bool SheathWeapons { get; set; }
        public bool TurnToFace { get; set; }
        public bool TextReplaceable { get; set; }
        public bool ChangesStandState { get; set; }
        public uint StandState { get; set; }
        public uint LocalizedTextIdCommand { get; set; }
        public uint LocalizedTextIdNotFoundToAll { get; set; }
        public uint LocalizedTextIdNotFoundToSelf { get; set; }
        public uint NotFoundAnim { get; set; }
        public uint TextReplaceAnim { get; set; }
        public uint ModelSequenceIdStandState { get; set; }
        public uint VisualEffectId { get; set; }
        public uint Flags { get; set; }
        public string UniversalCommand00 { get; set; }
        public string UniversalCommand01 { get; set; }
    }
}
