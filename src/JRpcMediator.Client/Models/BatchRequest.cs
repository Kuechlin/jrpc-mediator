using MediatR;
namespace JRpcMediator.Client.Models;

public class BatchRequest
{
    public BatchRequest(int id, IBaseRequest request)
    {
        Id = id;
        Request = request;
    }

    public int Id { get; set; }
    public IBaseRequest Request { get; set; }
}
