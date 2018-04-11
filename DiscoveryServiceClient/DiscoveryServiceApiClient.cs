using Keysight.CommunicationsFabric.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Interop
{
    public class DiscoveryServiceApiClient
    {
        const string DiscoveryServiceName = "Connectivity.InstrumentDiscoveryService";
        const int DefaultConnectionTimeout = 5000;

        public DiscoveryServiceApiClient()
        {
            Initialize();
        }

        private void Initialize()
        {
            RemoteConnection.Establish();
            CommunicationDirectory.WaitForServiceReady(DiscoveryServiceName, null, DefaultConnectionTimeout);
        }

        public KcfResponse CallApi(KcfRequest request)
        {
            KcfResponse response = null;
            var manager = CommunicationManager.Instance;
            if (manager != null)
            {
                try
                {
                    var result = manager.CallWait(manager.ServiceNode,
                                     DiscoveryServiceName,
                                     request.MethodName,
                                     request.Data,
                                     request.Timeout);
                    response = new KcfResponse { Data = result };
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            return response;
        }
    }

    public class KcfRequest
    {
        public int Timeout { get; set; } = 5000;

        public string MethodName { get; set; } = string.Empty;

        public ArgMap Data { get; set; } = new ArgMap();
    }

    public class KcfResponse
    {
        public ArgMap Data { get; set; }
    }
}
