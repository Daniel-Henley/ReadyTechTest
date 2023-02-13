namespace ReadyTechTest.Tests.Shared;

public class TestHelpers
{
    public static Mock<IHttpClientFactory> GetHttpClientFactory(Mock<HttpMessageHandler> messageHandler)
    {
        var httpClient = new HttpClient(messageHandler.Object);
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                         .Returns(httpClient);

        return httpClientFactory;
    }
}
