using System;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace netping {
    public class NetPing {

        /// <summary>
        /// Ping a host and return the ping time. Return -1 if the ping times out.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        public static int SimplePing(string hostname, int timeoutMs, bool verbose) {

            var ping = new Ping();
            var pingOptions = new PingOptions { 
                DontFragment = true
            };
            string data = "####################";
            try {
                PingReply reply = ping.Send(hostname, timeoutMs, Encoding.ASCII.GetBytes(data), pingOptions);
                if (reply.Status == IPStatus.Success) {
                    return (int)reply.RoundtripTime;
                } else {
                    return -1;
                }
            } catch (PingException ex) {
                Exception exToReport = ex;
                if (ex.InnerException != null) {
                    exToReport = ex.InnerException;
                }
                if (verbose) {
                    Console.Error.WriteLine("Ping-Error: {0}: {1}", hostname, exToReport.Message);
                }
                return -1;
            }
        }

        public static Task<int> SimplePingAsync(string hostname, int timeoutMS, bool verbose) {
            var t = new Task<int>(() => SimplePing(hostname, timeoutMS, verbose));
            t.Start();
            return t;
        }


    }
}
