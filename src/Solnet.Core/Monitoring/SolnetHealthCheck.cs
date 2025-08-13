using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Solnet.Core.Monitoring
{
    public class SolnetHealthCheck :
        IHealthCheck
    {
        readonly ISolnetInstance _busInstance;

        public SolnetHealthCheck(ISolnetInstance busInstance)
        {
            _busInstance = busInstance;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            var result = _busInstance.SolnetControl.CheckHealth();

            var data = new Dictionary<string, object>
            {
                ["Endpoints"] = new EndpointDictionary(result.Endpoints.ToDictionary(x => x.Key,
                    x => new Endpoint(Enum.GetName(typeof(SolnetHealthStatus), x.Value.Status), x.Value.Description)
                ))
            };

            var minimalHealthcheckLevel = context.Registration.FailureStatus switch
            {
                HealthStatus.Healthy => SolnetHealthStatus.Healthy,
                HealthStatus.Degraded => SolnetHealthStatus.Degraded,
                _ => SolnetHealthStatus.Unhealthy
            };

            var usedHealthcheckResult = result.Status < minimalHealthcheckLevel ? minimalHealthcheckLevel : result.Status;

            return Task.FromResult(usedHealthcheckResult switch
            {
                SolnetHealthStatus.Healthy => HealthCheckResult.Healthy(result.Description, data),
                SolnetHealthStatus.Degraded => HealthCheckResult.Degraded(result.Description, result.Exception, data),
                _ => HealthCheckResult.Unhealthy(result.Description, result.Exception, data)
            });
        }


        class EndpointDictionary :
            Dictionary<string, Endpoint>
        {
            public EndpointDictionary(IDictionary<string, Endpoint> dictionary)
                : base(dictionary, StringComparer.OrdinalIgnoreCase)
            {
            }

            public override string ToString()
            {
                return string.Join(", ", this.Select(x => $"{x.Key}: {x.Value}"));
            }
        }


        class Endpoint
        {
            public Endpoint(string status, string description)
            {
                Status = status;
                Description = description;
            }

            public string Status { get; set; }
            public string Description { get; set; }

            public override string ToString()
            {
                return $"{Status} - {Description}";
            }
        }
    }
}
