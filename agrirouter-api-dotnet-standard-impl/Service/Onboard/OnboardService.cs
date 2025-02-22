using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Agrirouter.Api.Dto.Onboard;
using Agrirouter.Api.Exception;
using Agrirouter.Api.Service.Parameters;
using Agrirouter.Impl.Service.Common;
using Newtonsoft.Json;
using Environment = Agrirouter.Api.Env.Environment;

namespace Agrirouter.Impl.Service.onboard
{
    /// <summary>
    ///     Service for the onboarding.
    /// </summary>
    public class OnboardService
    {
        private readonly Environment _environment;
        private readonly HttpClient _httpClient;
        private readonly UtcDataService _utcDataService;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="environment">The current environment.</param>
        /// <param name="utcDataService">The UTC data service.</param>
        /// <param name="httpClient">The current HTTP client.</param>
        public OnboardService(Environment environment, UtcDataService utcDataService, HttpClient httpClient)
        {
            _environment = environment;
            _httpClient = httpClient;
            _utcDataService = utcDataService;
        }

        /// <summary>
        ///     Onboard an endpoint using the simple onboarding procedure and the given parameters.
        /// </summary>
        /// <param name="onboardParameters">The onboarding parameters.</param>
        /// <returns>-</returns>
        /// <exception cref="OnboardException">Will be thrown if the onboarding was not successful.</exception>
        public OnboardResponse Onboard(OnboardParameters onboardParameters)
        {
            var onboardingRequest = new OnboardRequest
            {
                ExternalId = onboardParameters.Uuid,
                ApplicationId = onboardParameters.ApplicationId,
                CertificationVersionId = onboardParameters.CertificationVersionId,
                GatewayId = onboardParameters.GatewayId,
                CertificateType = onboardParameters.CertificationType,
                TimeZone = UtcDataService.TimeZone,
                UtcTimestamp = UtcDataService.Now
            };

            var requestBody = JsonConvert.SerializeObject(onboardingRequest);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_environment.OnboardingUrl()),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            httpRequestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", onboardParameters.RegistrationCode);

            var httpResponseMessage = _httpClient.SendAsync(httpRequestMessage).Result;

            if (!httpResponseMessage.IsSuccessStatusCode)
                throw new OnboardException(httpResponseMessage.StatusCode,
                    httpResponseMessage.Content.ReadAsStringAsync().Result);

            var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var onboardingResponse = JsonConvert.DeserializeObject(result, typeof(OnboardResponse));
            return onboardingResponse as OnboardResponse;
        }
    }
}