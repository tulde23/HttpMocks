# HttpMocks
_A Nuget package implementing Unit Testing "best practices"._



## Http Mocking

Practices.Testing provides a factory abstraction for mocking many aspects of the Http pipeline.
You can mock the following
1. DelegatingHandlers - DHs are the foundational blocks of all http client requests.  This is essentially how you mock the HttpClient.
2.  HttpClient - building upon the delegating handler mocking, you can achieve full control over your HttpClient.
3.   IHttpClientFactory - allows you to mock the creation of http clients.

### Http Examples

#### Mock A Delegating Handler


```csharp
 public class FakeDelegatingHandler : DelegatingHandler
    {
        public FakeDelegatingHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var builder = new UriBuilder(request.RequestUri);
            var content = await request.Content.ReadAsStringAsync();
            builder.Host = "www.google.com";
            var response = await base.SendAsync(request, cancellationToken);
            var data = await response.Content.ReadAsStringAsync();
            response.Content = new StringContent(data + content);
            return response;
        }
    }
```

And the test
```csharp
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
```

Using a fluent syntax, you "build up" your request, ultimately calling one of the Build methods to retrieve your mock.

#### Mock The IHttpClientFactory

```csharp
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
```



