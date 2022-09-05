using Example.Contract;
using JRpcMediator.Client;
using JRpcMediator.Client.Models;
using JRpcMediator.Server;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace JRpcMediator.Tests;

public class JRpcClientTests
{
    [Fact]
    public async void CanRequest()
    {
        var client = new WebApplicationFactory<Program>().CreateClient();

        var rpcClient = new JRpcClient(client, "/execute");

        var response = await rpcClient.Send(new DemoRequest("Max"));

        Assert.Equal("Hallo Max", response);
    }

    [Fact]
    public async void CanBatch()
    {
        var client = new WebApplicationFactory<Program>().CreateClient();

        var rpcClient = new JRpcClient(client, "/execute");

        var responses = await rpcClient.Batch(new[]
        {
            new BatchRequest(1, new DemoRequest("Max")),
            new BatchRequest(2, new DemoRequest("Mia")),
            new BatchRequest(3, new ErrorRequest("some error"))
        });

        var res1 = responses.FirstOrDefault(x => x.Id == 1);

        Assert.Equal("Hallo Max", res1?.Result);

        var res2 = responses.FirstOrDefault(x => x.Id == 2);

        Assert.Equal("Hallo Mia", res2?.Result);

        var res3 = responses.FirstOrDefault(x => x.Id == 3);

        Assert.Equal("some error", res3?.Exception?.Message);
    }
}