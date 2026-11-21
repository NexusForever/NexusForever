namespace NexusForever.Game.Abstract.Cinematic
{
    public interface IActorVisibility : IKeyframeAction
    {
        IActor Actor { get; }
        bool Hide { get; }
        bool AffectOnlyPlayers { get; }
    }
}
