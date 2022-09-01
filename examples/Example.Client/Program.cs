using Example.Contract;
using JRpcMediator;
using JRpcMediator.Client;



var client = new JRpcClient("http://localhost:6000/execute");

var response = await client.Send(new DemoRequest("Max"));

Console.WriteLine(response);

try
{
    await client.Send(new ErrorRequest("some error"));
}
catch (JRpcException e)
{
    Console.WriteLine("@{0}: {1}", e.RpcError.Type, e.RpcError.Message);
}
catch (Exception e)
{
    Console.WriteLine("@{0}: {1}", e.GetType().Name, e.Message);
}
