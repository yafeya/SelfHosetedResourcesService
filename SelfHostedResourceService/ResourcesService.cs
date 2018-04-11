using System;
using DiscoveryService.Interop;
using System.Collections.Generic;

namespace SelfHostedResourceService
{
    public class ResourcesService : IResourcesService
    {
        private IolsClient mClient = new IolsClient();
        
        public IEnumerable<Resource> GetResources()
        {
            return mClient.GetResources();
        }

        public ScpiResult SendReadCommand(string address, string command)
        {
            return mClient.SendReadCommand(address, command);
        }
    }
}
