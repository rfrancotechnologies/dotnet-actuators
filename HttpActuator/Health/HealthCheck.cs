using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Com.Rfranco.HttpActuator.Health
{
    /// <summary>
    /// Abstract definition HealthCheck
    /// </summary>
    public abstract class HealthCheck : IHealthCheck
    {
        public abstract Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken);

        public string Name {get;set;}

        public HealthCheck(string name)
        {
            this.Name = name;
        }
        
        public HealthCheckResult CreateHealthCheckResult(HealthStatus status, string details, Exception exception = null )
        {
            return new HealthCheckResult(status, $"{{\"checker\": \"{Name}\", \"status\": \"{status}\", \"details\": {details}}}" , exception);
        }
    }
}