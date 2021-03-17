using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using UnitTests.HttpMocking;
using Xunit;

namespace UnitTests
{
    public class TestHttpMocking
    {
        [Fact]
        public async Task DelegatingHandlerSingleTest()
        {
            var handler = HttpMock.Mock()
                                            .For()
                                            .Accepts("request")
                                            .Returns("response")
                                            .BuildDelegatingHandler(() => new FakeDelegatingHandler());
            var response = await handler.InvokeSingle();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.GetContentAsStringAsync();
            data.Should().Be("responserequest");
        }

        [Fact]
        public async Task DelegatingHandlerMultiTest()
        {
            var handler = HttpMock.Mock()
                                            .For("/test1")
                                            .Accepts("request1")
                                            .Returns("response1")
                                            .For("/test2")
                                            .Accepts("request2")
                                            .Returns("response2")
                                            .BuildDelegatingHandler(() => new FakeDelegatingHandler());
            var responses = await handler.Invoke();
            int counter = 1;
            foreach (var response in responses)
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var data = await response.GetContentAsStringAsync();
                data.Should().Be($"response{counter}request{counter}");
                counter++;
            }
        }

        [Fact]
        public async Task DelegatingHandlerMultiTestWithDuplicate()
        {
            var handler = HttpMock.Mock()
                                            .For("/test1")
                                            .Accepts("request1")
                                            .Returns("response1")
                                            .For("/test2")
                                            .Accepts("request2")
                                            .Returns("response2")
                                             .For("/test2")
                                            .Accepts("request2")
                                            .Returns("response2")
                                            .BuildDelegatingHandler(() => new FakeDelegatingHandler());
            var responses = await handler.Invoke();
            int counter = 1;
            foreach (var response in responses)
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var data = await response.GetContentAsStringAsync();
                data.Should().Be($"response{counter}request{counter}");
                counter++;
            }
        }

        [Fact]
        public async Task HttpClientTest()
        {
            var client = HttpMock.Mock()
                                            .For()
                                            .Accepts("request")
                                            .Returns("response")
                                            .BuildClient();
            var response = await client.PostAsync(string.Empty, It.IsAny<HttpContent>());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.GetContentAsStringAsync();
            data.Should().Be("response");
        }

        [Fact]
        public async Task HttpClientAnyTest()
        {
            var client = HttpMock.Mock()
                                            .For()
                                            .AcceptsAny()
                                            .Returns("response")
                                            .BuildClient();
            var response = await client.PostAsync(string.Empty, It.IsAny<HttpContent>());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.GetContentAsStringAsync();
            data.Should().Be("response");
        }

        [Fact]
        public async Task HttpClientTypedModelTest()
        {
            var client = HttpMock.Mock()
                                            .For()
                                            .Accepts(new FakeInput { Id = "1" })
                                            .Returns(new FakeOutput { Id = "1" })
                                            .BuildClient();
            var response = await client.PostAsync(string.Empty, It.IsAny<HttpContent>());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.GetContentAsStringAsync();
            var model = JsonConvert.DeserializeObject<FakeOutput>(data);
            model.Id.Should().Be("1");
        }

        [Fact]
        public async Task HttpClientFactoryTest()
        {
            var factory = HttpMock.Mock()
                                            .For()
                                            .Accepts("request")
                                            .Returns("response")
                                            .BuildFactory("test");

            using (var client = factory.CreateClient("test"))
            {
                var response = await client.PostAsync(string.Empty, It.IsAny<HttpContent>());
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var data = await response.GetContentAsStringAsync();
                data.Should().Be("response");
            }
        }

        [Fact]
        public async Task HttpClientFactoryTestWithEmptyName()
        {
            var factory = HttpMock.Mock()
                                            .For()
                                            .Accepts("request")
                                            .Returns("response")
                                            .BuildFactory();

            using (var client = factory.CreateClient())
            {
                var response = await client.PostAsync(string.Empty, It.IsAny<HttpContent>());
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var data = await response.GetContentAsStringAsync();
                data.Should().Be("response");
            }
        }

        public class FakeInput
        {
            public string Id { get; set; }
        }

        public class FakeOutput
        {
            public string Id { get; set; }
        }
    }
}