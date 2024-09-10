using System.Text.Json.Serialization;
using FluentValidation;

namespace Intaker.Contracts;

public class AddTodoValidator : AbstractValidator<AddTodoDto>
{
    public AddTodoValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty();
        RuleFor(r => r.Description)
            .NotEmpty();
    }
}
public record AddTodoDto(string Title, string Description, string? UpdatedBy);
public class UpdateTodoValidator : AbstractValidator<UpdateTodoDto>
{
    public UpdateTodoValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty();
        RuleFor(r => r.Description)
            .NotEmpty();
    }
}
public record UpdateTodoDto(string Title, string Description, TodoStatus Status, string? UpdatedBy);
public record TodoDto(
    string Name, 
    string Description,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]TodoStatus Status,
    string? AssignedTo,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy)
{
    public static TodoDto From(Todo todo) => new(todo.Name, todo.Description, todo.Status, todo.AssignedTo, todo.CreatedAt, todo.UpdatedAt, todo.UpdatedBy);
}