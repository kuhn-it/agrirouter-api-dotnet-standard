using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Agrirouter.Api.Builder;
using Agrirouter.Api.Dto.Messaging;
using Agrirouter.Api.Exception;
using Agrirouter.Api.Service.Messaging;
using Agrirouter.Api.Service.Parameters;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Serilog;

namespace Agrirouter.Impl.Service.Common
{
    /// <summary>
    ///     Service to send messages to the AR.
    /// </summary>
    public class MqttMessagingService : IMessagingService<MessagingParameters>
    {
        private readonly IMqttClient _mqttClient;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="mqttClient">-</param>
        public MqttMessagingService(IMqttClient mqttClient)
        {
            _mqttClient = mqttClient;
        }

        /// <summary>
        ///     Send message to the AR using the given message parameters.
        /// </summary>
        /// <param name="messagingParameters">Messaging parameters.</param>
        /// <returns>-</returns>
        /// <exception cref="CouldNotSendMqttMessageException">Will be thrown if the message could not be send.</exception>
        public MessagingResult Send(MessagingParameters messagingParameters)
        {
            var messageRequest = new MessageRequest
            {
                SensorAlternateId = messagingParameters.OnboardResponse.SensorAlternateId,
                CapabilityAlternateId = messagingParameters.OnboardResponse.CapabilityAlternateId,
                Messages = new List<Api.Dto.Messaging.Inner.Message>()
            };

            foreach (var message in messagingParameters.EncodedMessages.Select(encodedMessage =>
                new Api.Dto.Messaging.Inner.Message
                    {Content = encodedMessage, Timestamp = UtcDataService.NowAsUnixTimestamp()}))
                messageRequest.Messages.Add(message);

            var messagePayload = JsonConvert.SerializeObject(messageRequest);

            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(messagingParameters.OnboardResponse.ConnectionCriteria.Measures)
                .WithPayload(messagePayload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            _mqttClient.PublishAsync(mqttMessage, CancellationToken.None);

            return new MessagingResultBuilder().WithApplicationMessageId(messagingParameters.ApplicationMessageId)
                .Build();
        }
    }
}