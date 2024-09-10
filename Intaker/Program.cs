using FluentValidation;
using Intaker.Contracts;
using Intaker.Features;
using Intaker.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoContext>(c => c
    .UseNpgsql(builder.Configuration["Database:Connection"])
);
builder.Services
    .AddTransient<IValidator<UpdateTodoDto>, UpdateTodoValidator>()
    .AddTransient<IValidator<AddTodoDto>, AddTodoValidator>();
builder.Services
    .AddSingleton<IBus>(new ServiceBusHandler(builder.Configuration["ServiceBus:Host"]))
    .AddMediatR(c => c.RegisterServicesFromAssemblyContaining<AddTask>());

var app = builder.Build();

app.MapGet("/todos", async (/*pagination, etc, etc*/[FromServices] IMediator mediator) =>
{
    var result = await mediator.Send(new ListTodosQuery());
    
});
app.MapPost("/todos", async ([FromBody] AddTodoDto todo, [FromServices] IMediator mediator, [FromServices] IValidator<AddTodoDto> validator) =>
{
    var validationResult = validator.Validate(todo);
    if (!validationResult.IsValid)
        return Results.BadRequest(validationResult.ToDictionary());
    var result = await mediator.Send(new AddTaskCommand(todo.Title, todo.Description, todo.UpdatedBy));
    if (result.IsFailure)
        return Results.Problem(result.Error.Message);
    return Results.Ok(result.Value);
});
app.MapPut("/todos/{id}", async ([FromBody] UpdateTodoDto todo, [FromServices] IMediator mediator, int id) =>
{
    var result = await mediator.Send(new UpdateTodoCommand(id,todo.Title,todo.Status, todo.Description, todo.UpdatedBy));
    if (result.IsFailure)
        return result.Error switch
        {
            NotFoundError => Results.NotFound(),
            _ => Results.Problem(result.Error.Message)
        };
    return Results.Ok(result.Value);
});
app.Run();
