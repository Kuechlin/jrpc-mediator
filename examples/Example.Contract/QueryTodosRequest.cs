using Example.Contract.Models;
using JRpcMediator;
using MediatR;

namespace Example.Contract;

[JRpcMethod("query/todo")]
[JRpcAuthorize(Role = "reader")]
public class QueryTodosRequest : IRequest<TodoModel[]>
{
    public int Skip { get; set; }
    public int Take { get; set; }

    public QueryTodosRequest(int skip = 0, int take = 25)
    {
        Take = take;
        Skip = skip;
    }
}