using System;
using DiscoveryService.Interop;

namespace SelfHostedResourceService
{
    public class ResourcesService : IResourcesService
    {
        private IolsClient mClient = new IolsClient();
        
        public string GetResources()
        {
            return mClient.GetResources();
        }

        public string SendReadCommand(string address, string command)
        {
            return mClient.SendReadCommand(address, command);
        }
    }
}
