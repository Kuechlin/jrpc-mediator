using Microsoft.AspNetCore.Http;

namespace JRpcMediator.Server.Handlers;

public class JRpcBatchRequestHandler
{
    private readonly JRpcRequestHandler requestHandler;
    private readonly JRpcNotificationHandler notificationHandler;

    public JRpcBatchRequestHandler(JRpcRequestHandler requestHandler, JRpcNotificationHandler notificationHandler)
    {
        this.requestHandler = requestHandler;
        this.notificationHandler = notificationHandler;
    }

    public async Task<JRpcResponse[]> Handle(HttpContext context, JRpcRequest[] requests)
    {
        await Task.WhenAll(requests.Where(x => x.IsNotification()).Select(x => notificationHandler.Handle(context, x)));

        return await Task.WhenAll(requests.Where(x => x.IsRequest()).Select(x => requestHandler.Handle(context, x)));
    }
}
