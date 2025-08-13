using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Solnet.Core.Monitoring;
using System.Reflection;

namespace Solnet.Core.Configuration
{
    public class ConfigureSolnetHealthCheckServiceOptions :
           IConfigureOptions<HealthCheckServiceOptions>
    {
        readonly IEnumerable<ISolnetInstance> _solnetInstances;
        readonly IServiceProvider _provider;
        readonly string[] _tags;

        public ConfigureSolnetHealthCheckServiceOptions(IEnumerable<ISolnetInstance> solnetInstances, IServiceProvider provider)
        {
            _solnetInstances = solnetInstances;
            _provider = provider;
            _tags = new[] { "ready", "masstransit" };
        }

        public void Configure(HealthCheckServiceOptions options)
        {
            foreach (var solnetInstance in _solnetInstances)
            {
                var type = typeof(SolnetCoreHealthCheckOptions<>).MakeGenericType(solnetInstance.InstanceType);
                var optionsType = typeof(IOptions<>).MakeGenericType(type);

                var name = solnetInstance.Name;
                HealthStatus? minimalFailureStatus = HealthStatus.Unhealthy;
                var tags = new HashSet<string>(_tags, StringComparer.OrdinalIgnoreCase);

                var solnetOptions = _provider.GetService(optionsType);
                if (solnetOptions != null)
                {
                    var healthCheckOptions = (IHealthCheckOptions?)optionsType?.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public)?
                        .GetValue(solnetOptions, null);

                    if (healthCheckOptions != null)
                    {
                        if (!string.IsNullOrWhiteSpace(healthCheckOptions.Name))
                            name = healthCheckOptions.Name;

                        if (healthCheckOptions.MinimalFailureStatus.HasValue)
                            minimalFailureStatus = healthCheckOptions.MinimalFailureStatus.Value;

                        if (healthCheckOptions.Tags.Count != 0)
                            tags = healthCheckOptions.Tags;
                    }
                }

                options.Registrations.Add(new HealthCheckRegistration(name, new SolnetHealthCheck(solnetInstance), minimalFailureStatus, tags));
            }
        }
    }
}