namespace Tracker.Api.Demo.Extensions;

public static class UsersRouteExtensions
{
    public static RouteGroupBuilder MapUserApi(this RouteGroupBuilder group)
    {
        group.MapGet("/adming", () => "test");
        group.MapGet("/{id}", () => "test");

        return group;
    }
}
