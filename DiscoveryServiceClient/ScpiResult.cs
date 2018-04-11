using Keysight.CommunicationsFabric.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Interop
{
    class ScpiResult
    {
        public bool Succeed { get; set; } = false;
        public string Message { get; set; } = string.Empty;

        public ArgMap ToArgMap()
        {
            var argmap = new ArgMap();

            argmap["Succeed"] = Succeed;
            argmap["Message"] = Message;

            return argmap;
        }
    }
}
