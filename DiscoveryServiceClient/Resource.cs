using Keysight.CommunicationsFabric.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Interop
{
    class Resource
    {
        public Resource() : this(null)
        { }

        public Resource(ArgMap argmap)
        {
            if (argmap != null)
            {
                PersistentId = argmap.GetString("PersistentId", string.Empty);
                Manufacturer = argmap.GetString("Manufacturer", string.Empty);
                Model = argmap.GetString("Model", string.Empty);
                SerialNumber = argmap.GetString("SerialNumber", string.Empty);
                Firmware = argmap.GetString("Firmware", string.Empty);
                Description = argmap.GetString("Description", string.Empty);
                VisaAddress = argmap.GetString("VisaAddress", string.Empty);
                SiclAddress = argmap.GetString("SiclAddress", string.Empty);
            }
        }

        public string PersistentId { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Firmware { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VisaAddress { get; set; } = string.Empty;
        public string SiclAddress { get; set; } = string.Empty;

        public ArgMap ToArgMap()
        {
            var argmap = new ArgMap();
            argmap["Id"]            = PersistentId;
            argmap["Manufacturer"]  = Manufacturer;
            argmap["Model"]         = Model;
            argmap["SerialNumber"]  = SerialNumber;
            argmap["Firmware"]      = Firmware;
            argmap["Description"]   = Description;
            argmap["VisaAddress"]   = VisaAddress;
            argmap["SiclAddress"]   = SiclAddress;
            return argmap;
        }
    }
}
