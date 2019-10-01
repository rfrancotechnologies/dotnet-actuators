using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Com.RFranco.HttpActuator
{
    public static class HealthStatusExtension
    {
        public const string STATUS_PASS = "pass";
        public const string STATUS_WARNING = "warning";
        public const string STATUS_FAIL = "fail";
        
        /// <summary>
        /// https://tools.ietf.org/id/draft-inadarei-api-health-check-02.html
        /// </summary>
        /// <param name="healthStatus"></param>
        /// <returns></returns>
        public static string GetValue(this HealthStatus healthStatus)
        {
            return healthStatus.Equals(HealthStatus.Healthy) ? STATUS_PASS : 
                healthStatus.Equals(HealthStatus.Degraded) ? STATUS_WARNING : STATUS_FAIL;
        }        
    }
}

