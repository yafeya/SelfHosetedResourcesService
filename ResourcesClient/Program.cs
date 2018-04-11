using ResourcesClient.ResourcesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ResourcesServiceClient();
            var resources = client.GetResources();
            Console.WriteLine(resources);
            Console.ReadKey();
        }
    }
}
