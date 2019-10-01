using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Rfranco.HttpActuator
{
    /// <summary>
    /// Abstract definition of an Http Actuator
    /// </summary>
    public abstract class HttpActuator
    {
        public string Endpoint { get; set; }

        protected HttpActuator(string endpoint)
        {
            this.Endpoint = endpoint;
        }
        
        public abstract Task ProcessRequest(HttpListenerResponse response, CancellationToken cancel);

        protected void WriteResponse(HttpListenerResponse response, int statusCode = 200, string body = null)
        {
            response.ContentType = "application/json";
            response.StatusCode = statusCode;
            if (body != null)
            {
                var writer = new StreamWriter(response.OutputStream);
                writer.Write(body);
                writer.Flush();
                writer.Close();
            }
        }
    }
}