using CSharpFunctionalExtensions;
using Intaker.Contracts;
using Intaker.Infrastructure;
using MediatR;

namespace Intaker.Features;

public record UpdateTodoCommand(int Id, string Name, string Description, string? UpdatedBy) : IRequest<Result<TodoDto, Error>>;
public class UpdateTask : IRequestHandler<UpdateTodoCommand, Result<TodoDto, Error>>
{
    private readonly TodoContext _context;

    public UpdateTask(TodoContext context)
    {
        _context = context;
    }
    public async Task<Result<TodoDto, Error>> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _context.Todos.FindAsync(request.Id);
        if(todo is null)
            return Result.Failure<TodoDto, Error>(new NotFoundError($"Todo {request.Id} not found"));
        todo.Update(request.Name, request.Description, request.UpdatedBy);
        return await Save(todo)
            .Map(() => TodoDto.From(todo));
    }
    private async Task<UnitResult<Error>> Save(Todo todo)
    {
        try
        {
            _context.Todos.Update(todo);
            await _context.SaveChangesAsync();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            return new InfrastructureError("Failed to save todo");
        }
    }
}