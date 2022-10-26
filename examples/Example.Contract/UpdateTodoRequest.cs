using Example.Contract.Models;
using JRpcMediator;
using MediatR;

namespace Example.Contract;

[JRpcMethod("update/todo")]
[JRpcAuthorize(Role = "writer")]
public class UpdateTodoRequest : IRequest<TodoModel>
{
    public TodoModel Model { get; set; }

    public UpdateTodoRequest(TodoModel model)
    {
        Model = model;
    }
}