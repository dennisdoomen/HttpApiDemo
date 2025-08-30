using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;

namespace HttpApiDemo.Specs;

internal static class HttpClientExtensions
{
    public static HttpResponseMessageAssertions Should(this HttpResponseMessage response)
    {
        return new HttpResponseMessageAssertions(response);
    }
}


internal class HttpResponseMessageAssertions(HttpResponseMessage response)
{
    public async Task BeEquivalentTo<T>(T expectation)
    {
        string body = await response.Content.ReadAsStringAsync();

        object actual = JsonSerializer.Deserialize(body, expectation.GetType(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

        actual.Should().BeEquivalentTo(expectation);
    }
}
