using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Entities
{
    public class Basket
    {
        public int Id { get; set; }
        public string IPAdress { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }

        //Navigation'lar

        public string UserId { get; set; }

        public List<BasketDetail> BasketDetails { get; set; }
    }
}
