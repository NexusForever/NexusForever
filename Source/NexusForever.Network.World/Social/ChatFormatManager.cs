using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.Game.Static.Social;
using NexusForever.Shared;

namespace NexusForever.Network.World.Social;

public sealed class ChatFormatManager : Singleton<ChatFormatManager>, IChatFormatManager
{
    private delegate IChatFormat ChatFormatFactoryDelegate();
    private ImmutableDictionary<ChatFormatType, ChatFormatFactoryDelegate> chatFormatFactories;

    public void Initialise()
    {
        var builder = ImmutableDictionary.CreateBuilder<ChatFormatType, ChatFormatFactoryDelegate>();

        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            ChatFormatAttribute attribute = type.GetCustomAttribute<ChatFormatAttribute>();
            if (attribute == null)
                continue;

            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                continue;

            NewExpression @new = Expression.New(constructor);
            ChatFormatFactoryDelegate factory = Expression.Lambda<ChatFormatFactoryDelegate>(@new).Compile();
            builder.Add(attribute.Type, factory);
        }

        chatFormatFactories = builder.ToImmutable();
    }

    /// <summary>
    /// Returns a new <see cref="IChatFormat"/> model for supplied <see cref="ChatFormatType"/> type.
    /// </summary>
    public IChatFormat NewChatFormatModel(ChatFormatType type)
    {
        return chatFormatFactories.TryGetValue(type, out ChatFormatFactoryDelegate factory) ? factory.Invoke() : null;
    }
}
