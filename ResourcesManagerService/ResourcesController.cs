using DiscoveryService.Interop;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ResourcesManagerService
{
    public class ResourcesController : ApiController
    {

        private IolsClient mIolsClient = new IolsClient();

        public IEnumerable<Resource> Get()
        {
            return mIolsClient.GetResources();
        }

        public string Get(int id)
        {
            return string.Empty;
        }
        
        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }

        [HttpPost]
        public ScpiResult SendReadCommand(CommandRequest request)
        {
            var address = request.Address;
            var command = request.Command;
            var result = mIolsClient.SendReadCommand(address, command);
            return result;
        }
    }
}
