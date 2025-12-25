using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Extensions;

public static class EndpointBuilderExtensions
{
    public static TBuilder WithTracking<TBuilder, TContext>(this TBuilder endpoint, GlobalOptions options)
        where TBuilder : IEndpointConventionBuilder
        where TContext : DbContext
    {
        return endpoint.AddEndpointFilterFactory((provider, next) =>
        {
            var builder = provider.ApplicationServices.GetRequiredService<IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>>();
            var immutableOptions = builder.Build<TContext>(options);

            var etagService = provider.ApplicationServices.GetRequiredService<IRequestHandler>();
            var requestFilter = provider.ApplicationServices.GetRequiredService<IRequestFilter>();

            var filter = new TrackerEndpointFilter(etagService, requestFilter, immutableOptions);
            return (context) => filter.InvokeAsync(context, next);
        });
    }

    public static TBuilder WithTracking<TBuilder, TContext>(this TBuilder endpoint, Action<GlobalOptions> configure)
        where TBuilder : IEndpointConventionBuilder
        where TContext : DbContext
    {
        var options = new GlobalOptions();
        configure(options);
        return endpoint.WithTracking<TBuilder, TContext>(options);
    }

    public static TBuilder WithTracking<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder =>
        builder.AddEndpointFilter<TBuilder, TrackerEndpointFilter>();

    public static TBuilder WithTracking<TBuilder>(this TBuilder endpoint, GlobalOptions options)
        where TBuilder : IEndpointConventionBuilder
    {
        return endpoint.AddEndpointFilterFactory((provider, next) =>
        {
            var builder = provider.ApplicationServices.GetRequiredService<IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>>();
            var immutableOptions = builder.Build(options);

            var etagService = provider.ApplicationServices.GetRequiredService<IRequestHandler>();
            var requestFilter = provider.ApplicationServices.GetRequiredService<IRequestFilter>();

            var filter = new TrackerEndpointFilter(etagService, requestFilter, immutableOptions);
            return (context) => filter.InvokeAsync(context, next);
        });
    }

    public static TBuilder WithTracking<TBuilder>(this TBuilder builder, Action<GlobalOptions> configure)
        where TBuilder : IEndpointConventionBuilder
    {
        var options = new GlobalOptions();
        configure(options);
        return builder.WithTracking(options);
    }
}
