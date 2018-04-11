using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesManagerService
{
    public class CommandRequest
    {
        public string Address { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
    }
}
