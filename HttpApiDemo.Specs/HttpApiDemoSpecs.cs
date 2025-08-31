using System.Net;
using System.Net.Http;
using System.Text;
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

    [Fact]
    public async Task Can_upload_package_and_track_status_to_completion()
    {
        // Arrange
        var client = httpFactory.CreateClient();
        var packageData = """
                          {
                              "Id": "TestPackage",
                              "Versions": [
                                  {
                                      "Version": "1.0.0",
                                      "Description": "Test package description",
                                      "RepositoryUrl": "https://github.com/test/package",
                                      "Owner": "testowner"
                                  }
                              ]
                          }
                          """;

        // Upload package
        HttpResponseMessage uploadResponse = await client.PostAsync("api/v2/packages",
            new StringContent(packageData, Encoding.UTF8, "application/json"));

        uploadResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        uploadResponse.Headers.Location.Should().NotBeNull();

        string statusUrl = uploadResponse.Headers.Location!.ToString();
        statusUrl.Should().Contain("/status/");

        // Use the status link to get the status (first call returns InProgress and marks completion)
        HttpResponseMessage statusResponse = await client.GetAsync(statusUrl);
        statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        string statusBody = await statusResponse.Content.ReadAsStringAsync();
        statusBody.Should().Contain("InProgres");

        // After the first status check, the package should now be available (the repository implementation
        // marks the package as completed after the first status check, clearing the PendingId)
        statusResponse = await client.GetAsync(statusUrl);
        statusResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        string packageUrl = statusResponse.Headers.Location?.ToString();
        packageUrl.Should().ContainEquivalentOf("TestPackage");

        // Package information is returned correctly
        HttpResponseMessage packageResponse = await client.GetAsync(packageUrl);
        packageResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        await packageResponse.Should().BeEquivalentTo(new
        {
            Id = "TestPackage",
            Versions = new[]
            {
                new
                {
                    Version = "1.0.0",
                    Description = "Test package description",
                    RepositoryUrl = "https://github.com/test/package",
                    Owner = "testowner"
                }
            }
        });
    }
}
