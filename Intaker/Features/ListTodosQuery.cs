using CSharpFunctionalExtensions;
using Intaker.Contracts;
using Intaker.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Intaker.Features;

public record ListTodosQuery : IRequest<Result<TodoDto[], Error>>;
public class ListTodos : IRequestHandler<ListTodosQuery, Result<TodoDto[], Error>>
{
    private readonly TodoContext _context;

    public ListTodos(TodoContext context)
    {
        _context = context;
    }
    public async Task<Result<TodoDto[], Error>> Handle(ListTodosQuery request, CancellationToken cancellationToken)
    {
        var todos = await _context.Todos.ToListAsync();
        var dtos =todos.Select(TodoDto.From).ToArray();
        return Result.Success<TodoDto[], Error>(dtos);
    }
}