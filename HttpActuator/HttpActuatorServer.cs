using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Rfranco.HttpActuator
{
    /// <summary>
    /// Interface IHttpActuatorServer
    /// </summary>
    public interface IHttpActuatorServer : IDisposable
    {
        IHttpActuatorServer Start();

        Task StopAsync();

        void Stop();

        void Add(HttpActuator actuator);

        void AddRange(List<HttpActuator> actuators);

    }

    /// <summary>
    /// Implementation of IHttpActuatorServer
    /// </summary>
    public class HttpActuatorServer : IHttpActuatorServer, IDisposable
    {
        private readonly HttpListener _httpListener = new HttpListener();

        private CancellationTokenSource _cts = new CancellationTokenSource();

        private Task _task;

        private int Port;

        private string HostName;

        private bool UseHttps;

        private readonly ConcurrentDictionary<string, HttpActuator> _actuators = new ConcurrentDictionary<string, HttpActuator>();

        public HttpActuatorServer(int port, string host = "+", bool useHttps = false)
        {
            this.Port = port;
            this.HostName = host;
            this.UseHttps = useHttps;

        }

        public void Add(HttpActuator actuator)
        {
            var s = UseHttps ? "s" : "";
            _httpListener.Prefixes.Add($"http{s}://{HostName}:{Port}/{actuator.Endpoint}/");
            _actuators[actuator.Endpoint] = actuator;
        }

        public void AddRange(List<HttpActuator> actuators)
        {
            foreach (HttpActuator actuator in actuators)
                Add(actuator);
        }


        public void Dispose()
        {
            Stop();
        }

        public IHttpActuatorServer Start()
        {
            if (_task != null)
                throw new InvalidOperationException("The http server has already been started.");

            _task = StartServer(_cts.Token);
            return this;
        }

        public void Stop()
        {
            StopAsync().GetAwaiter().GetResult();
        }

        public async Task StopAsync()
        {
            _cts?.Cancel();

            try
            {
                if (_task == null)
                    return; // Never started.

                await _task;
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        protected Task StartServer(CancellationToken cancel)
        {
            _httpListener.Start();

            return Task.Factory.StartNew(async delegate
            {
                try
                {
                    while (!cancel.IsCancellationRequested)
                    {
                        await DispatchRequest(cancel);
                    }
                }
                finally
                {
                    _httpListener.Stop();
                    _httpListener.Close();
                }
            }, TaskCreationOptions.LongRunning);
        }

        private async Task DispatchRequest(CancellationToken cancel)
        {
            var getContext = _httpListener.GetContextAsync();
            getContext.Wait(cancel);
            var context = getContext.Result;
            var request = context.Request;
            var response = context.Response;

            try
            {
                var dispatcher = GetDispatcher(request);
                if(null != dispatcher)
                    await dispatcher.ProcessRequest(response, cancel);
                else
                    response.StatusCode = 404;
            }
            catch (Exception)
            {
                response.StatusCode = 500;                
            }
            finally
            {
                response.Close();
            }
        }

        private HttpActuator GetDispatcher(HttpListenerRequest request)
        {
            var endpoint = request.RawUrl.Replace("/", "");
            return _actuators.ContainsKey(endpoint) ? _actuators[endpoint] : null;
        }
    }
}