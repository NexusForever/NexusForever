using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity.Movement.Command.Mode;
using NexusForever.Game.Abstract.Entity.Movement.Command.Move;
using NexusForever.Game.Abstract.Entity.Movement.Command.Platform;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;
using NexusForever.Game.Abstract.Entity.Movement.Command.Rotation;
using NexusForever.Game.Abstract.Entity.Movement.Command.Scale;
using NexusForever.Game.Abstract.Entity.Movement.Command.State;
using NexusForever.Game.Abstract.Entity.Movement.Command.Time;
using NexusForever.Game.Abstract.Entity.Movement.Command.Velocity;
using NexusForever.Game.Entity.Movement.Command.Mode;
using NexusForever.Game.Entity.Movement.Command.Move;
using NexusForever.Game.Entity.Movement.Command.Platform;
using NexusForever.Game.Entity.Movement.Command.Position;
using NexusForever.Game.Entity.Movement.Command.Rotation;
using NexusForever.Game.Entity.Movement.Command.Scale;
using NexusForever.Game.Entity.Movement.Command.State;
using NexusForever.Game.Entity.Movement.Command.Time;
using NexusForever.Game.Entity.Movement.Command.Velocity;
using NexusForever.Shared;

namespace NexusForever.Game.Entity.Movement.Command
{
    public static class ServiceCollectionExtentions
    {
        public static void AddGameEntityMovementCommand(this IServiceCollection sc)
        {
            sc.AddTransient<IModeCommandGroup, ModeCommandGroup>();
            sc.AddSingleton<IFactoryInterface<IModeCommand>, FactoryInterface<IModeCommand>>();
            sc.AddTransient<ModeCommand>();
            sc.AddTransient<ModeDefaultCommand>();
            sc.AddTransient<ModeKeysCommand>();

            sc.AddTransient<IMoveCommandGroup, MoveCommandGroup>();
            sc.AddSingleton<IFactoryInterface<IMoveCommand>, FactoryInterface<IMoveCommand>>();
            sc.AddTransient<MoveCommand>();
            sc.AddTransient<MoveDefaultsCommand>();
            sc.AddTransient<MoveKeysCommand>();

            sc.AddTransient<IPlatformCommandGroup, PlatformCommandGroup>();
            sc.AddSingleton<IFactoryInterface<IPlatformCommand>, FactoryInterface<IPlatformCommand>>();
            sc.AddTransient<PlatformCommand>();

            sc.AddTransient<IPositionCommandGroup, PositionCommandGroup>();
            sc.AddSingleton<IFactoryInterface<IPositionCommand>, FactoryInterface<IPositionCommand>>();
            sc.AddTransient<PositionCommand>();
            sc.AddTransient<PositionKeysCommand>();
            sc.AddTransient<PositionPathCommand>();
            sc.AddTransient<PositionSplineCommand>();

            sc.AddTransient<IRotationCommandGroup, RotationCommandGroup>();
            sc.AddSingleton<IFactoryInterface<IRotationCommand>, FactoryInterface<IRotationCommand>>();
            sc.AddTransient<RotationCommand>();
            sc.AddTransient<RotationDefaultsCommand>();
            sc.AddTransient<RotationFacePositionCommand>();
            sc.AddTransient<RotationFaceUnitCommand>();
            sc.AddTransient<RotationKeysCommand>();
            sc.AddTransient<RotationSpinCommand>();

            sc.AddTransient<IScaleCommandGroup, ScaleCommandGroup>();
            sc.AddSingleton<IFactoryInterface<IScaleCommand>, FactoryInterface<IScaleCommand>>();
            sc.AddTransient<ScaleCommand>();
            sc.AddTransient<ScaleKeysCommand>();

            sc.AddTransient<IStateCommandGroup, StateCommandGroup>();
            sc.AddSingleton<IFactoryInterface<IStateCommand>, FactoryInterface<IStateCommand>>();
            sc.AddTransient<StateCommand>();
            sc.AddTransient<StateDefaultCommand>();
            sc.AddTransient<StateKeysCommand>();

            sc.AddTransient<ITimeCommandGroup, TimeCommandGroup>();
            sc.AddSingleton<IFactoryInterface<ITimeCommand>, FactoryInterface<ITimeCommand>>();
            sc.AddTransient<TimeCommand>();

            sc.AddTransient<IVelocityCommandGroup, VelocityCommandGroup>();
            sc.AddSingleton<IFactoryInterface<IVelocityCommand>, FactoryInterface<IVelocityCommand>>();
            sc.AddTransient<VelocityCommand>();
            sc.AddTransient<VelocityDefaultsCommand>();
        }
    }
}
