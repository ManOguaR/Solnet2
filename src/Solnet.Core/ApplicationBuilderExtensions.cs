using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Solnet.Core.Builder;

namespace Solnet.Core
{
    public static class ApplicationBuilderExtensions
    {
        public static IHostApplicationBuilder AddSolnet(this IHostApplicationBuilder applicationBuilder)
        {
            applicationBuilder.AddSolnetCore();
            return applicationBuilder;
        }

        public static ISolnetCoreBuilder AddSolnetCore(this IHostApplicationBuilder applicationBuilder)
        {
            return null;
        }

        public static ISolnetCoreBuilder AddSolnetCore(this IServiceCollection services)
        {
            return null;
        }
    }
}
