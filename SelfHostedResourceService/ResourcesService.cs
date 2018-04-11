using Keysight.CommunicationsFabric.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SelfHostedResourceService
{
    public class ResourcesService : IResourcesService
    {
        private DiscoveryServiceClient mClient = new DiscoveryServiceClient();

        public string GetResources()
        {
            string json = Consts.EmptyJsonArray;
            var request = new KcfRequest
            {
                MethodName = Consts.GetSnapshot_Method,
                Data = new ArgMap()
            };
            var response = mClient.CallApi(request);

            var devicesRaw = response.Data[Consts.Devices];
            if (devicesRaw != null && devicesRaw is ArgList)
            {
                var devicesListRaw = devicesRaw as ArgList;
                json = Json.Encode(devicesListRaw, 4);
            }

            return json;
        }
    }
}
