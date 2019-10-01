using Com.Rfranco.HttpActuator.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.SqlServer
{
    public class SqlServerHealthCheck : HealthCheck
    {
        private readonly string ConnectioString;
        
        public SqlServerHealthCheck(string connectionString, string name = "SqlServer") : base (name)
        {
            ConnectioString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));            
        }
        public override async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectioString))
                {
                    await connection.OpenAsync(cancellationToken);
                    return CreateHealthCheckResult(HealthStatus.Healthy, $"{{\"serverVersion:\": \"{connection.ServerVersion}\", \"workStationId\":\"{connection.WorkstationId}\"}}");
                }
            }
            catch (Exception ex)
            {
                return CreateHealthCheckResult(HealthStatus.Unhealthy, $"{{\"error\":\"{ex.Message}\"}}", ex);                
            }
        }
    }
}
