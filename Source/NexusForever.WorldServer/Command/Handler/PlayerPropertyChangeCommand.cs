using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class PlayerPropertyChangeCommand : NamedCommand
    {
        private readonly PropertyInfo property;

        public PlayerPropertyChangeCommand(Expression<Func<Player, float>> propertyLambda, params string[] aliases)
            : base(true, aliases)
        {
            property = GetPropertyFromExpression(propertyLambda);
            if (!property.CanRead || !property.CanWrite)
                throw new ArgumentException("Property specified must be readable and writable to use this base class.",
                    nameof(propertyLambda));
        }

        protected override async Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 1)
            {
                await SendHelpAsync(context);
                return;
            }

            if (!float.TryParse(parameters[0], out float newValue) || newValue < 0.0f)
            {
                await SendHelpAsync(context);
                return;
            }

            float originalValue = SetPropertyValue(context, newValue);
            context.Session.Player.JumpHeight = newValue;
            await context.SendMessageAsync($"{property.Name} set to {newValue}, previous value: {originalValue}");
        }

        protected virtual float GetPropertyValue(CommandContext context)
        {
            return (float) property.GetValue(context.Session.Player);
        }

        protected virtual float SetPropertyValue(CommandContext context, float newValue)
        {
            float originalValue = GetPropertyValue(context);
            property.SetValue(context.Session.Player, newValue);
            return originalValue;
        }

        private PropertyInfo GetPropertyFromExpression<T>(Expression<Func<T, float>> propertyLambda)
        {
            MemberExpression memberExpression = null;

            //this line is necessary, because sometimes the expression comes in as Convert(originalexpression)
            if (propertyLambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression) propertyLambda.Body;
                if (unaryExpression.Operand is MemberExpression)
                    memberExpression = (MemberExpression) unaryExpression.Operand;
                else
                    throw new ArgumentException();
            }
            else if (propertyLambda.Body is MemberExpression)
                memberExpression = (MemberExpression) propertyLambda.Body;
            else
                throw new ArgumentException();

            return (PropertyInfo) memberExpression.Member;
        }
    }
}
