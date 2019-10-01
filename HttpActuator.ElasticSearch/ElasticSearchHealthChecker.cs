using System;
using System.Threading;
using System.Threading.Tasks;
using Com.Rfranco.HttpActuator.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nest;

namespace Com.Rfranco.HttpActuator.Health.ElasticSearch
{
    public class ElasticSearchHealthChecker : HealthCheck
    {
        private IElasticClient ElasticClient;

        public ElasticSearchHealthChecker(IElasticClient client, string name = "elasticsearch") : base(name)
        {
            this.ElasticClient = client;
        }

        public override async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            try
            {
                var pingResult = await ElasticClient.PingAsync(cancellationToken: cancellationToken);
                return CreateHealthCheckResult(pingResult.ApiCall.HttpStatusCode == 200 ? HealthStatus.Healthy : HealthStatus.Unhealthy,
                    $"{{\"uri:\": \"{pingResult.ApiCall.Uri}\", \"debugInformation\":\"{pingResult.ApiCall.DebugInformation}\"}}");
            }
            catch (Exception ex)
            {
                return CreateHealthCheckResult(HealthStatus.Unhealthy, $"{{\"error\":\"{ex.Message}\"}}", ex);                
            }
        }
    }
}
