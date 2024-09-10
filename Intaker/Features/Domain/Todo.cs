
public enum TodoStatus
{
    NotStarted,
    InProgress,
    Completed
}
public class Todo
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public TodoStatus Status { get; private set; }
    public string? AssignedTo { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    public static Todo New(string name, string description, string? assignedTo)
    {
        return new Todo()
        {
            Name = name,
            Description = description,
            Status = TodoStatus.NotStarted,
            CreatedAt = DateTimeOffset.UtcNow,
            AssignedTo = assignedTo
        };
    }

    public void Update(string name, string description, string? updatedBy)
    {
        Name = name;
        Description = description;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}