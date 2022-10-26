using Example.Contract;
using Example.Contract.Models;
using Example.Server.Data;
using JRpcMediator.Exceptions;
using MediatR;

namespace Example.Server.Handlers;

public class GetTodoRequestHandler : IRequestHandler<GetTodoRequest, TodoModel>
{
    private readonly TodoContext context;

    public GetTodoRequestHandler(TodoContext context)
    {
        this.context = context;
    }

    public async Task<TodoModel> Handle(GetTodoRequest request, CancellationToken cancellationToken)
    {
        var entity = await context.Todos.FindAsync(new object?[] { request.Id }, cancellationToken);

        if (entity == null)
            throw new JRpcNotFoundException("Todo not found");

        return entity;
    }
}