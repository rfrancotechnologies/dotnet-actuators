using Com.Rfranco.HttpActuator.Health;
using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Rfranco.HttpActuator.Health.Kafka
{
    public class KafkaHealthChecker : HealthCheck
    {
        private readonly AdminClientConfig Configuration;
        private AdminClientBuilder AdminClientBuilder;
        public KafkaHealthChecker(AdminClientConfig configuration, string name = "ApacheKafka") : base(name)
        {
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.Name = name;
            this.AdminClientBuilder = new AdminClientBuilder(Configuration);            
        }
        public override async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            return await Task<HealthCheckResult>.Run(() =>
            {
                try
                {
                    using (var adminClient = AdminClientBuilder.Build())
                    {
                        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                        return CreateHealthCheckResult(HealthStatus.Healthy, $"{{\"OriginatingBrokerId\":\"{metadata.OriginatingBrokerId}\", \"OriginatingBrokerName\":\"{metadata.OriginatingBrokerName}\"}}");
                    }
                }
                catch (Exception ex)
                {
                    return CreateHealthCheckResult(HealthStatus.Unhealthy, $"{{\"error\":\"{ex.Message}\"}}", ex);
                }

            });
        }
    }
}
