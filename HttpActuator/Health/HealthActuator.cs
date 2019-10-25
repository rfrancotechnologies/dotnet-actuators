using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Com.RFranco.HttpActuator;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Com.Rfranco.HttpActuator.Health
{
    public class HealthActuator : HttpActuator
    {
        private readonly ConcurrentDictionary<string, HealthCheck> Checkers;

        public HealthActuator(ConcurrentDictionary<string, HealthCheck> checkers = null, string endpoint="health") : base(endpoint)
        {
            this.Checkers = checkers ?? new ConcurrentDictionary<string, HealthCheck>();
        }
        public override async Task ProcessRequest(HttpListenerResponse response, CancellationToken cancel)
        {
            try
            {
                await CheckHealthAsync(response, cancel);
            }
            catch (Exception)
            {
                WriteResponse(response,(int) HttpStatusCode.InternalServerError, $"{{\"status\" : \"{HealthStatus.Unhealthy.GetValue()}\"}}");
            }
        }

        public void AddChecker(HealthCheck checker)
        {
            Checkers[checker.Name] = checker;
        }

        private Task CheckHealthAsync(HttpListenerResponse response, CancellationToken cancel)
        {
            List<Task<HealthCheckResult>> tasks = new List<Task<HealthCheckResult>>();
            foreach (var checker in Checkers.Values)
                tasks.Add(checker.CheckHealthAsync(null, cancel));

            var results = tasks.Select(x => x.Result);
            var details = tasks.Select(x => x.Result.Description);

            HealthStatus status = results.Where(result => result.Status.Equals(HealthStatus.Unhealthy)).Count() > 0 
                ? HealthStatus.Unhealthy :  results.Where(result => result.Status.Equals(HealthStatus.Degraded)).Count() > 0 ?
                HealthStatus.Degraded : HealthStatus.Healthy;

            WriteResponse(response, 
                (status.Equals(HealthStatus.Unhealthy)) ?  (int) HttpStatusCode.ServiceUnavailable : (int) HttpStatusCode.OK, 
                $"{{\"status\" : \"{status.GetValue()}\",  \"details\": [{details.ToCsv()}]}}");
            
            return null;
        }
    }
}