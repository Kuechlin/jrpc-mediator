using Example.Contract;
using Example.Contract.Models;
using Example.Server.Data;
using JRpcMediator.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Example.Server.Handlers;

public class UpdateTodoRequestHandler : IRequestHandler<UpdateTodoRequest, TodoModel>
{
    private readonly TodoContext context;

    public UpdateTodoRequestHandler(TodoContext context)
    {
        this.context = context;
    }

    public async Task<TodoModel> Handle(UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        if (await context.Todos.AnyAsync(x => x.Id == request.Model.Id, cancellationToken) is false)
            throw new JRpcNotFoundException("Todo not found");

        context.Todos.Update(request.Model);

        await context.SaveChangesAsync(cancellationToken);

        return request.Model;
    }
}
