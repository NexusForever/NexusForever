namespace NexusForever.Game.Abstract.Entity.Movement.Spline
{
    public interface ISplinePointLength
    {
        float Distance { get; set; }
        float T { get; set; }
        float Delay { get; set; }
    }
}