using DiscoveryService.Interop;
using System;
using System.ServiceModel;

namespace SelfHostedResourceService
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new ServiceHost(typeof(ResourcesService)))
            {
                if (host.State != CommunicationState.Opened)
                {
                    host.Open();
                }
                Console.WriteLine("Server Started...");

                Console.ReadKey();
            }
        }
    }
}
