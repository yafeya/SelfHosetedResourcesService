using Agilent.TMFramework.InstrumentIO;
using Keysight.CommunicationsFabric.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Interop
{
    public class IolsClient
    {
        private DiscoveryServiceApiClient mClient = new DiscoveryServiceApiClient();

        public IEnumerable<Resource> GetResources()
        {
            var request = new KcfRequest
            {
                MethodName = KnownStrings.GetSnapshot_Method,
                Data = new ArgMap()
            };
            var response = mClient.CallApi(request);

            var resourceList = new List<Resource>();

            var devicesRaw = response.Data[KnownStrings.Devices] as ArgList;

            if (devicesRaw != null)
            {
                FillResourceList(resourceList, devicesRaw);
            }

            var chassisesRaw = response.Data[KnownStrings.Chassis] as ArgList;

            if (chassisesRaw != null)
            {
                FillResourceList(resourceList, chassisesRaw);
            }

            return resourceList;
        }

        public ScpiResult SendReadCommand(string address, string command)
        {
            var commandCommunicator = new CommandCommunicator();
            var result = commandCommunicator.SendReadCommand(address, command);
            return result;
        }

        private static void FillResourceList(List<Resource> resourceList, ArgList rawList)
        {
            foreach (var raw in rawList)
            {
                var deviceRaw = raw as ArgMap;
                if (deviceRaw != null)
                {
                    var resource = new Resource(deviceRaw);
                    resourceList.Add(resource);
                }
            }
        }
    }
}
