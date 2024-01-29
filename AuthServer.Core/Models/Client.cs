using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Models
{

    //Models'i uygulama içi lazım olan herhangi objeler için kullanıyoruz.
    public class Client
    {
        public string Id { get; set; }
        public string Secret { get; set; }


        // www.miniapp1.com www.miniapp2.com
        public List<String> Audiences { get; set; }
    }
}
