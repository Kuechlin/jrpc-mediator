using Example.Contract.Models;
using JRpcMediator;
using MediatR;

namespace Example.Contract
{
    [JRpcMethod("create/todo")]
    [JRpcAuthorize(Role = "writer")]
    public class CreateTodoRequest : IRequest<TodoModel>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public CreateTodoRequest(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}