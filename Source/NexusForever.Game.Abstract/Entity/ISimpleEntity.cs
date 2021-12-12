namespace NexusForever.Game.Abstract.Entity
{
    public interface ISimpleEntity : IUnitEntity
    {
        byte QuestChecklistIdx { get; }

        void Initialise(uint creatureId, Action<ISimpleEntity> actionAfterAddToMap = null);
    }
}
