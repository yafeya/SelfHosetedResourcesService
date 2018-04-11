using Keysight.CommunicationsFabric.Protocol;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

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
