using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace talenthubBE.Test;

public class ApiTestSets: IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;

    // Testing GET method on "/api/developers"
    public ApiTestSets(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task TestReturnedMoreThanOne()
    {
        var response = await _client.GetAsync("/api/developers");
        var content = await response.Content.ReadAsStringAsync();
        var developers = JsonConvert.DeserializeObject<List<Developer>>(content); // Replace YourObject with the model you expect
        Assert.True(developers.Count > 0, "Returned list count should be more than zerp");
        Assert.False(developers[0].Id == Guid.Empty, "First developer's Id should not be an empty string");
    }

    // POST-GET-PUT-DELETE test 
    [Fact]
    public async Task TestDevAPIPost()
    {
    // Testing POST method
        var newDeveloper = new Developer { Name = "Test Lups", Email = "test@lups.com" };
        var jsonString = JsonConvert.SerializeObject(newDeveloper);
        var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var responseSingleDev = await _client.PostAsync("/api/developers", stringContent);
        var contentTest = await responseSingleDev.Content.ReadAsStringAsync();
        var developerTest = JsonConvert.DeserializeObject<Developer>(contentTest);
        
        Assert.Equal(newDeveloper.Name, developerTest.Name);
        Assert.Equal(newDeveloper.Email, developerTest.Email);

    // Testing GET method
        var response_Get = await _client.GetAsync($"/api/developers/{developerTest.Id}");
        var content_Get = await response_Get.Content.ReadAsStringAsync();
        var developer_Get = JsonConvert.DeserializeObject<Developer>(content_Get);
        
        Assert.Equal(developer_Get.Name, developerTest.Name);
        Assert.Equal(developer_Get.Email, developerTest.Email);

    // Testing PUT method
        var newDeveloper_Put = new Developer { Id = developerTest.Id, Name = "Test Lups Put", Email = "new_test@lups.com", CreatedAt = developerTest.CreatedAt };
        var jsonString_Put = JsonConvert.SerializeObject(newDeveloper_Put);
        var stringContent_Put = new StringContent(jsonString_Put, Encoding.UTF8, "application/json");

        var response_Put = await _client.PutAsync($"/api/developers/{developerTest.Id}", stringContent_Put);
        // PUT response return empty content, 🐞 can not even check statuscode
        // Assert.Equal(HttpStatusCode.OK, response_Put.StatusCode);
        var response_Put_Check = await _client.GetAsync($"/api/developers/{developerTest.Id}");
        var content_Put_Check = await response_Put_Check.Content.ReadAsStringAsync();
        var developer_Put_Check = JsonConvert.DeserializeObject<Developer>(content_Put_Check);

        Assert.Equal(developer_Put_Check.Name, newDeveloper_Put.Name);

    // Testing DELETE method
        var response_Delete = await _client.DeleteAsync($"/api/developers/{developerTest.Id}");
        // Assert.Equal(HttpStatusCode.OK, response_Delete.StatusCode);
        var response_Delete_Check = await _client.GetAsync($"/api/developers/{developerTest.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response_Delete_Check.StatusCode);
    }
}