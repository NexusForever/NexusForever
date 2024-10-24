namespace NexusForever.Game.Abstract.Entity
{
    public interface ISimpleEntity : IUnitEntity
    {
        void Initialise(uint creatureId, Action<ISimpleEntity> actionAfterAddToMap = null);
    }
}
