using Agrirouter.Api.Service.Messaging;
using Agrirouter.Api.Service.Parameters;
using Agrirouter.Impl.Service.Messaging.Abstraction;
using Agrirouter.Request;

namespace Agrirouter.Impl.Service.Messaging
{
    /// <summary>
    ///     Service to publish multiple messages.
    /// </summary>
    public class PublishMultipleMessagesService : SendMultipleMessagesBaseService
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="messagingService">-</param>
        public PublishMultipleMessagesService(IMessagingService<MessagingParameters> messagingService) : base(
            messagingService)
        {
        }

        protected override RequestEnvelope.Types.Mode Mode => RequestEnvelope.Types.Mode.Publish;
    }
}