using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Entities
{
    public class BasketDetail
    {
        public int Quantity { get; set; }
        public int Price { get; set; }


        public Basket Basket { get; set; }
        public int BasketId { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}
