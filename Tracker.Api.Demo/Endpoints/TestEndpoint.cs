using FastEndpoints;
using Tracker.FastEndpoints;
using Tracker.FastEndpoints.Attributes;

namespace Tracker.Api.Demo.Endpoints;

[TrackerOptions(tables: ["roles"], cacheControl: "max-age=60, stale-while-revalidate=60, stale-if-error=86400")]
public class MyEndpoint : Endpoint<EmptyRequest>
{
    public override void Configure()
    {
        AllowAnonymous();
        Get("/sales/orders/create");
        PreProcessor<TrackerPreProcessor<EmptyRequest>>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        await Send.OkAsync(new { Success = true }, ct);
    }
}
