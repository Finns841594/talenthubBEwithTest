using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace talenthubBE.Test;

public class ApiTestSets: IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;
    private  HttpResponseMessage _response;
    private  List<Developer> _developers;
    public ApiTestSets(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        SetUp().Wait();
    }
    private async Task SetUp()
    {
        _response = await _client.GetAsync("/api/developers");
        var content = await _response.Content.ReadAsStringAsync();
        _developers = JsonConvert.DeserializeObject<List<Developer>>(content);
    }

    [Fact]
    public void TestResponseOk()
        {
            Assert.Equal("OK", _response.StatusCode.ToString());
        }

        [Fact]
        public void TestReturnedMoreThanOne()
        {
            Assert.True(_developers.Count > 0, "Returned list count should be more than zero");
        }

    [Fact]
    public void TestFirstIdNotEmpty()
    {
        Assert.False(_developers[0].Id == Guid.Empty, "First developer's Id should not be an empty string");
    }
    [Fact]
    public async Task TestDevAPIPost()
    {
    // Arrange
    var newDeveloper = new Developer { Name = "Test Lups", Email = "test@lups.com" };
    var jsonString = JsonConvert.SerializeObject(newDeveloper);
    var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

    // Act
    var response = await _client.PostAsync("/api/developers", stringContent);

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    
    var content = await response.Content.ReadAsStringAsync();
    var developer = JsonConvert.DeserializeObject<Developer>(content);
    
    // Add your additional checks here. For example, you might check that the
    // developer returned in the response has the same properties as the one you sent:
    Assert.Equal(newDeveloper.Name, developer.Name);
    Assert.Equal(newDeveloper.Email, developer.Email);
    }
}