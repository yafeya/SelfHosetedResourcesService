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

        public string GetResources()
        {
            string json = KnownStrings.EmptyJsonArray;
            var request = new KcfRequest
            {
                MethodName = KnownStrings.GetSnapshot_Method,
                Data = new ArgMap()
            };
            var response = mClient.CallApi(request);
            var resourceList = new ArgList();

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

            json = Json.Encode(resourceList, 4);

            return json;
        }

        public string SendReadCommand(string address, string command)
        {
            var commandCommunicator = new CommandCommunicator();
            var result = commandCommunicator.SendReadCommand(address, command);
            var arg = result.ToArgMap();
            var json = Json.Encode(arg, 4);
            return json;
        }

        private static void FillResourceList(ArgList resourceList, ArgList rawList)
        {
            foreach (var raw in rawList)
            {
                var deviceRaw = raw as ArgMap;
                if (deviceRaw != null)
                {
                    var resource = new Resource(deviceRaw);
                    resourceList.Add(resource.ToArgMap());
                }
            }
        }
    }
}
