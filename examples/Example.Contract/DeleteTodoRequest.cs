using Example.Contract.Models;
using JRpcMediator;
using MediatR;

namespace Example.Contract;

[JRpcMethod("delete/todo")]
[JRpcAuthorize(Role = "writer")]
public class DeleteTodoRequest : IRequest
{
    public int Id { get; set; }

    public DeleteTodoRequest(int id)
    {
        Id = id;
    }
}
