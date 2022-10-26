using Example.Contract;
using Example.Contract.Models;
using Example.Server.Data;
using JRpcMediator;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Example.Server.Handlers;

public class QueryTodosRequestHandler : IRequestHandler<QueryTodosRequest, TodoModel[]>
{
    private readonly TodoContext context;

    public QueryTodosRequestHandler(TodoContext context)
    {
        this.context = context;
    }

    public async Task<TodoModel[]> Handle(QueryTodosRequest request, CancellationToken cancellationToken)
    {
        return await context
            .Todos
            .AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .ToArrayAsync(cancellationToken);
    }
}
