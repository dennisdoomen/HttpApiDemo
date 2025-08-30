using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace HttpApiDemo.Specs;

public class HttpApiDemoSpecs(CustomWebApplicationFactory httpFactory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Can_find_a_package_by_id()
    {
        // Arrange
        var client = httpFactory.CreateClient();

        // Act
        string body = await client.GetStringAsync("api/packages/pathy");

        var definition = new
        {
            Id = "",
            Versions = new[]
            {
                new
                {
                    Version = "",
                    Description = "",
                    RepositoryUrl = "",
                    Owner = ""
                }
            },
        };

        var result = JsonSerializer.Deserialize(body, definition.GetType(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        result.Should().BeEquivalentTo(new
        {
            Id = "Pathy",
            Versions = new[]
            {
                new
                {
                    Description = Value.ThatMatches<string>(x => x.Contains("path")),
                    Owner = "dennisdoomen",
                    RepositoryUrl = "https://github.com/dennisdoomen/pathy",
                    Version = "1.5.0"
                },
                new
                {
                    Description = Value.ThatMatches<string>(x => x.Contains("path")),
                    Owner = "dennisdoomen",
                    RepositoryUrl = "https://github.com/dennisdoomen/pathy",
                    Version = "1.0.0"
                }
            }
        });
    }

    [Fact]
    public async Task Can_find_a_package_by_id_v2()
    {
        // Arrange
        var client = httpFactory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("api/packages/pathy");

        // Assert
        await response.Should().BeEquivalentTo(new
        {
            Id = "Pathy",
            Versions = new[]
            {
                new
                {
                    Description = "Fluently building and using file and directory paths without binary dependencies",
                    Owner = "dennisdoomen",
                    RepositoryUrl = "https://github.com/dennisdoomen/pathy",
                    Version = "1.5.0"
                },
                new
                {
                    Description = "Fluently building and using file and directory paths without binary dependencies",
                    Owner = "dennisdoomen",
                    RepositoryUrl = "https://github.com/dennisdoomen/pathy",
                    Version = "1.0.0"
                }
            }
        });
    }

}

