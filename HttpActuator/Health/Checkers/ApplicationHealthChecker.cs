using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Com.RFranco.HttpActuator;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Com.Rfranco.HttpActuator.Health
{
    public class ApplicationHealthChecker : HealthCheck
    {
        private ApplicationHealthCheckerOption Options;
        public ApplicationHealthChecker(ApplicationHealthCheckerOption options, string name = "Application") : base(name)
        {
            this.Options = options;            
        }

        public override async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            var errors = await Task.FromResult<IEnumerable<ExceptionReport>>(ApplicationExceptionMetrics.ExceptionsReported
                .Where(e => e.WhenHappend <= now && e.WhenHappend >= now.Subtract(TimeSpan.FromSeconds(Options.IntervaleOfActitivySeconds))));
            
            return CreateHealthCheckResult(GetStatus(errors), $"[{errors.Select(e => e.ToString()).ToCsv()}]");
        }

        private HealthStatus GetStatus(IEnumerable<ExceptionReport> errors)
        {
            return ((errors.Count() > Options.MaxNumberOfExceptionSupported) ? HealthStatus.Unhealthy : 
                (errors.Count() >= (Options.MaxNumberOfExceptionSupported / 2) + 1) ? HealthStatus.Degraded : HealthStatus.Healthy);
        }
    }
}