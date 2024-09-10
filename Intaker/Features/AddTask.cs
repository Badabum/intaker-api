using CSharpFunctionalExtensions;
using Intaker.Contracts;
using Intaker.Infrastructure;
using MediatR;

namespace Intaker.Features;

public record AddTaskCommand(string Name, string Description, string? UpdatedBy) : IRequest<Result<TodoDto, Error>>;

public class AddTask : IRequestHandler<AddTaskCommand, Result<TodoDto, Error>>
{
    private readonly TodoContext _context;

    public AddTask(TodoContext context)
    {
        _context = context;
    }
    public async Task<Result<TodoDto, Error>> Handle(AddTaskCommand request, CancellationToken cancellationToken)
    {
        var todo = Todo.New(request.Name, request.Description, request.UpdatedBy);
        return await Save(todo)
            .Map(() => TodoDto.From(todo));
    }

    private async Task<UnitResult<Error>> Save(Todo todo)
    {
        try
        {
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            return new InfrastructureError("Failed to save todo");
        }
    }
}