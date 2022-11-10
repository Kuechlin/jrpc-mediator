using Example.Contract.Models;
using JRpcMediator;
using MediatR;

namespace Example.Contract
{
    [JRpcMethod("get/todo")]
    [JRpcAuthorize(Role = "reader")]
    public class GetTodoRequest : IRequest<TodoModel>
    {
        public int Id { get; set; }

        public GetTodoRequest(int id)
        {
            Id = id;
        }
    }
}