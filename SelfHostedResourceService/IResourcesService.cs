﻿using DiscoveryService.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SelfHostedResourceService
{
    [ServiceContract]
    public interface IResourcesService
    {
        [OperationContract]
        IEnumerable<Resource> GetResources();

        [OperationContract]
        ScpiResult SendReadCommand(string address, string command);
    }
}
