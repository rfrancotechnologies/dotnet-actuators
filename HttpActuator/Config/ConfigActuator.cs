using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Com.RFranco.HttpActuator;
using Microsoft.Extensions.Configuration;

namespace Com.Rfranco.HttpActuator.Config
{
    /// <summary>
    /// Retrieve the configuration of the component
    /// </summary>
    public class ConfigActuator : HttpActuator
    {
        protected readonly IConfiguration Configuration;

        public ConfigActuator(IConfiguration configuration, string endpoint = "config") : base(endpoint)
        {
            this.Configuration = configuration;
        }
        public override async Task ProcessRequest(HttpListenerResponse response, CancellationToken cancel)
        {
            try
            {
                string strDetails = string.Empty;
                var keysValues = await Task.FromResult<List<KeyValuePair<string,string>>>(Configuration.AsEnumerable().ToList());
                WriteResponse(response, (int) HttpStatusCode.OK, 
                    $"{{\"configuration\": [{keysValues.Select(keyValue => $"{{\"property\" : \"{keyValue.Key}\", \"value\" : \"{HttpUtility.JavaScriptStringEncode(keyValue.Value)}\"}}").ToCsv()}]}}");
            }
            catch (Exception)
            {
                response.StatusCode = (int) HttpStatusCode.InternalServerError;                
            }
        }
    }
}