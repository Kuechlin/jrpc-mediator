using Example.Contract;
using Example.Contract.Models;
using Example.Server.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Example.Server.Handlers;

public class CreateTodoRequestHandler : IRequestHandler<CreateTodoRequest, TodoModel>
{
    private readonly TodoContext context;

    public CreateTodoRequestHandler(TodoContext context)
    {
        this.context = context;
    }

    public async Task<TodoModel> Handle(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var entity = new TodoModel
        {
            Name = request.Name,
            Description = request.Description
        };

        context.Todos.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}
