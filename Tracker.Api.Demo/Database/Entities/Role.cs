namespace Tracker.Api.Demo.Database.Entities;

public sealed class Role
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
