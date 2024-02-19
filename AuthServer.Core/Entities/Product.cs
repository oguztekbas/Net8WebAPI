using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Decimal Price { get; set; }

        List<BasketDetail> BasketDetails { get; set;}
        //Navigationlar
        

    }
}
