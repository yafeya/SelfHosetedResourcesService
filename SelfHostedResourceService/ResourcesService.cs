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
        public string GetResources()
        {
            return "Hello world!~";
        }
    }
}
