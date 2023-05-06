namespace NexusForever.Game.Abstract.Cinematic
{
    public interface ICinematicFactory
    {
        ICinematicBase CreateCinematic<T>() where T : ICinematicBase;
    }
}
