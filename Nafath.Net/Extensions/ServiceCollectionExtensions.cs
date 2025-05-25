using Microsoft.Extensions.DependencyInjection;
using Nafath.Net.Interfaces;
using Nafath.Net.Services;
using System;

namespace Nafath.Net.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNafath(this IServiceCollection services, Action<NafathOptions> configure)
    {
        services.Configure(configure);
        services.AddHttpClient<INafathService, NafathService>();
        return services;
    }
}
