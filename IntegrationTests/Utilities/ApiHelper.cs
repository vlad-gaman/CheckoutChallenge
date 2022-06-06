using Refit;
using System;

namespace IntegrationTests.Utilities
{
    public static class ApiHelper
    {
        public static T CreateApiClient<T>()
        {
            var endpointUrl = GetEndpoint();
            return RestService.For<T>(endpointUrl);
        }

        private static string GetEndpoint()
        {
            return Environment.GetEnvironmentVariable("CheckoutApi");
        }
    }
}
