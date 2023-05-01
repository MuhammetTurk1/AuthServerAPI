using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Configuration
{
    public class Client
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public List<String>Audiences { get; set; }
        //Bu Client hangi apilere ,üyelik sistemi olmayan hangi apilere 
        //erişebilir bunu burdaki audiences kısmından belirticez
    }
}
