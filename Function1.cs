using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientFunctionApp
{
    public class MyFunction
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<MyFunction> _logger;

        public MyFunction(IHttpClientFactory clientFactory, ILogger<MyFunction> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [Function("MyFunction")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
            FunctionContext context)
        {
            var response = req.CreateResponse();

            var client = _clientFactory.CreateClient();

            // Example: Sending a GET request
            var httpResponse = await client.GetAsync("https://example.com");

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseBody = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response: {responseBody}");

                // Set the response content asynchronously
                await response.WriteStringAsync(responseBody, Encoding.UTF8);
            }
            else
            {
                _logger.LogError($"Failed to get data. Status code: {httpResponse.StatusCode}");
                await response.WriteStringAsync("Failed to fetch data.", Encoding.UTF8);
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
}
