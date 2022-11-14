using Example.Contract;
using Example.Contract.Models;
using JRpcMediator.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Text.Json;

namespace JRpcMediator.Tests;

public class JRpcClientTests
{
    private static JRpcClient GetClient()
    {
        var client = new WebApplicationFactory<Program>().CreateClient();

        var options = new JRpcClientOptions
        {
            Url = "/execute",
            JsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        };

        return new JRpcClient(options, client);
    }
    private static async Task Login(JRpcClient client)
    {
        // login
        var token = await client.Send(new LoginRequest("admin", "root"));
        // set token
        client.Configure(client => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token));
    }

    [Fact]
    public async void CanLogin()
    {
        // Arrange
        var client = GetClient();
        // Act
        var response = await client.Send(new LoginRequest("admin", "root"));
        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async void CanBatch()
    {
        // Arrange
        var client = GetClient();

        await Login(client);

        // Act
        var responses = await client.Batch(new()
        {
            { 1, new CreateTodoRequest("1", "Hello world!")},
            { 2, new CreateTodoRequest("2", "Hello world!")},
            { 3, new CreateTodoRequest("3", "Hello world!")},
            { 4, new ResultRequest("hello", "world") }
        });

        // Assert
        var assert = (string name, IResult res) =>
        {
            Assert.IsType<TodoModel>(res.Value);
            Assert.Equal(name, ((TodoModel)res.Value!).Name);
        };

        Assert.NotNull(responses);
        assert("1", responses[1]);
        assert("2", responses[2]);
        assert("3", responses[3]);
        
        Assert.IsType<Result<Dictionary<string, string>>>(responses[4]);
        var dict = (Dictionary<string, string>)responses[4]!.Value!;
        Assert.Equal("world", dict["hello"]);
    }

    [Fact]
    public async void CanGetSuccessResult()
    {
        // Arrange
        var client = GetClient();
        // Act
        var response = await client.Send(new ResultRequest("text", "hello, world"));
        // Assert
        Assert.NotNull(response);
        Assert.IsType<Result<Dictionary<string, string>>>(response);
        var dict = (Dictionary<string, string>)response!.Value!;
        Assert.Equal("text", dict.Keys.First());
        Assert.Equal("hello, world", dict["text"]);
    }

    [Fact]
    public async void CanGetFailureResult()
    {
        // Arrange
        var client = GetClient();
        // Act
        var response = await client.Send(new ResultRequest("text", "hello, world", true));
        // Assert
        Assert.NotNull(response);
        Assert.IsType<Result<Dictionary<string, string>>>(response);
        Assert.NotNull(response.Exception);
    }
}