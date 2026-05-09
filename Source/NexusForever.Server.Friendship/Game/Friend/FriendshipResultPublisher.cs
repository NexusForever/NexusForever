using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Friendship;
using InternalIdentity = NexusForever.Network.Internal.Message.Shared.Identity;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public class FriendshipResultPublisher
    {
        /// <summary>
        /// Publishes the result of a friendship operation.
        /// </summary>
        /// <param name="messagePublisher">The message publisher to send the result.</param>
        /// <param name="target">The target identity for the result message.</param>
        /// <param name="task">The task that produces the friendship result.</param>
        public async Task PublishResultAsync(IInternalMessagePublisher messagePublisher, InternalIdentity target, Task<FriendshipResult?> task)
        {
            FriendshipResult? result = await task;
            if (result != null)
            {
                await messagePublisher.PublishAsync(new FriendshipResultMessage
                {
                    Target = target,
                    Result = result.Value
                });
            }
        }
    }
}
