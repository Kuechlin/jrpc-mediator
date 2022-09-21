using Example.Contract;
using JRpcMediator;
using JRpcMediator.Client;
using static Example.Client.Utils;


var handler = new HttpClientHandler();

if (Question("credentials?"))
{
    handler.UseDefaultCredentials = true;
}

var client = new JRpcClient(new HttpClient(handler), "http://localhost:5000/execute");

do
{
    Console.WriteLine("select method: demo / error / secret?");

    try
    {
        string? response = null;
        switch (Console.ReadLine())
        {
            case "demo":
                Console.WriteLine("name?");
                var name = Console.ReadLine();
                response = await client.Send(new DemoRequest(name ?? "Max"));
                break;
            case "error":
                await client.Send(new ErrorRequest("some error"));
                break;
            case "secret":
                response = await client.Send(new SecretRequest("secret text"));
                break;
            case "exit":
                Environment.Exit(0);
                break;
        }
        if (response != null)
            Console.WriteLine(response);
    }
    catch (JRpcException e)
    {
        Console.WriteLine("@{0}: {1}", e.RpcError.Type, e.RpcError.Message);
    }
    catch (Exception e)
    {
        Console.WriteLine("@{0}: {1}", e.GetType().Name, e.Message);
    }
}
while(true);