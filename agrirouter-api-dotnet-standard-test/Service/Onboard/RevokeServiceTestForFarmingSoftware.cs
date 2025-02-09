using System;
using System.Collections.Generic;
using System.Net.Http;
using Agrirouter.Api.Exception;
using Agrirouter.Api.Service.Parameters;
using Agrirouter.Api.Test.Helper;
using Agrirouter.Impl.Service.onboard;
using Xunit;

namespace Agrirouter.Api.Test.Service.Onboard
{
    /// <summary>
    ///     Functional tests.
    /// </summary>
    [Collection("Integrationtest")]
    public class RevokeServiceTestForFarmingSoftware : AbstractSecuredIntegrationTestForFarmingSoftware
    {
        private static readonly HttpClient HttpClient = HttpClientFactory.HttpClient();

        private static string AccountId => "5d47a537-9455-410d-aa6d-fbd69a5cf990";

        private new static string ApplicationId => "16b1c3ab-55ef-412c-952b-f280424272e1";

        private static string EndpointId => "72c57d24-7d38-4607-b9f3-c3afbe8473db";

        [Fact(Skip = "Will only run if there is an endpoint with the given endpoint ID.")]
        public void GivenExistingEndpointWhenRevokingThenTheEndpointShouldBeRevoked()
        {
            var revokeParameters = new RevokeParameters
            {
                AccountId = AccountId,
                EndpointIds = new List<string> {EndpointId},
                ApplicationId = ApplicationId
            };

            var revokeService = new RevokeService(Environment, HttpClient);
            revokeService.Revoke(revokeParameters, PrivateKey);
        }

        [Fact]
        public void GivenNonExistingEndpointWhenRevokingThenTheEndpointShouldBeRevoked()
        {
            var revokeParameters = new RevokeParameters
            {
                AccountId = AccountId,
                EndpointIds = new List<string> {Guid.NewGuid().ToString()},
                ApplicationId = ApplicationId
            };

            var revokeService = new RevokeService(Environment, HttpClient);
            Assert.Throws<RevokeException>(() => revokeService.Revoke(revokeParameters, PrivateKey));
        }
    }
}