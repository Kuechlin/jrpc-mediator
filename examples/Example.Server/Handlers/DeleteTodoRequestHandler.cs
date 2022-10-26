using Example.Contract;
using Example.Server.Data;
using JRpcMediator.Exceptions;
using MediatR;

namespace Example.Server.Handlers;

public class DeleteTodoRequestHandler : IRequestHandler<DeleteTodoRequest>
{
    private readonly TodoContext context;

    public DeleteTodoRequestHandler(TodoContext context)
    {
        this.context = context;
    }

    public async Task<Unit> Handle(DeleteTodoRequest request, CancellationToken cancellationToken)
    {
        var entity = await context.Todos.FindAsync(new object?[] { request.Id }, cancellationToken);

        if (entity == null)
            throw new JRpcNotFoundException("Todo not found");

        context.Todos.Remove(entity);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
