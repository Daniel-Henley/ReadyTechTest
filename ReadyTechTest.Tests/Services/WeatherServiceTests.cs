using Moq.Protected;
using Newtonsoft.Json;
using ReadyTechTest.API.Services;
using ReadyTechTest.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReadyTechTest.Tests.Services
{
    public class WeatherServiceTests
    {
        [Fact]
        public async void WeatherServiceReturnsExpectedValueAsync()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var mockHttpClientFactory = TestHelpers.GetHttpClientFactory(mockHttpMessageHandler);

            var lat = "1";
            var lon = "2";
            var expectedValue = 20;
            var tempResponse = new { main = new { temp = expectedValue } };
            var tempResponseString = JsonConvert.SerializeObject(tempResponse);

            mockHttpMessageHandler.Protected()
                                  .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                                    ItExpr.IsAny<HttpRequestMessage>(),
                                                                    ItExpr.IsAny<CancellationToken>())
                                  .ReturnsAsync(new HttpResponseMessage
                                  {
                                      StatusCode = HttpStatusCode.OK,
                                      Content = new StringContent(tempResponseString)
                                  });

            var weatherService = new WeatherService(mockHttpClientFactory.Object);
            var response = await weatherService.GetWeatherByLocationAsync(lat, lon);
            response.ShouldBe(expectedValue);
        }
    }
}
