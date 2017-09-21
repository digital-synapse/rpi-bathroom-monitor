using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rpi_bathroom_monitor
{
    public class HttpService : IDisposable
    {
        private static List<HttpService> services = new List<HttpService>();
        public static void Start(Func<HttpListenerRequest, string> method, string route, int port = 80, string ipaddress = null)
        {
            var svc = new HttpService(method, route,port,ipaddress);
            services.Add(svc);
            svc.Run();
        }

        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public HttpService(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
            _listener.Start();
        }

        public HttpService(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }

        public HttpService(Func<HttpListenerRequest, string> method, string route, int port=80, string ipaddress=null) : this(new[] { $"http://{ipaddress ?? GetLocalIPv4()}:{port}{FormatRoute(route)}" }, method) { this.route = route; }
        private string route;

        public void Run()
        {
            Console.WriteLine("get " + route);
            ThreadPool.QueueUserWorkItem((o) =>
            {                
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        private static string getLocalIPv4(NetworkInterfaceType? _type = null)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((_type == null || item.NetworkInterfaceType == _type) && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }

        public static string GetLocalIPv4()
        {
            var ip = getLocalIPv4(NetworkInterfaceType.Ethernet);
            if (string.IsNullOrEmpty(ip))
                ip = getLocalIPv4(NetworkInterfaceType.Wireless80211);
            if (string.IsNullOrEmpty(ip))
                ip = getLocalIPv4();

            return ip;
        }

        public static string FormatRoute(string route)
        {
            if (!route.StartsWith("/")) route = "/" + route;
            if (!route.EndsWith("/")) route = route + "/";
            return route;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var svc in services)
                    {
                        svc.Stop();                        
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HttpService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
