using System;
using System.Threading;
using System.Threading.Tasks;
using Com.Rfranco.HttpActuator.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace HttpActuator.Redis
{
    public class RedisHealthChecker : HealthCheck
    {
        private readonly Lazy<ConnectionMultiplexer> Redis;

        public RedisHealthChecker(string nodes, string name = "redis") : base(name)
        {
            Redis = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(nodes));
        }

        public override async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            try
            {
                var timespan = await Redis.Value.GetDatabase().PingAsync();
                return CreateHealthCheckResult(HealthStatus.Healthy, $"{{\"latencyMs:\": \"{timespan.Milliseconds}\"}}");
            }
            catch (Exception ex)
            {
                return CreateHealthCheckResult(HealthStatus.Unhealthy, $"{{\"error\":\"{ex.Message}\"}}", ex);
            }
        }
    }
}