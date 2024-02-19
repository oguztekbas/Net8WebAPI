using AuthServer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class BasketDto
    {
        public BasketDto()
        {
            BasketDetails = new List<BasketDetailDto>();
        }
        public int Id { get; set; }
        public string? IPAdress { get; set; }
        public string? Address { get; set; }
        public DateTime Date { get; set; }

        public decimal TotalPrice { get; set; }


        public string? UserId { get; set; }

        public List<BasketDetailDto> BasketDetails { get; set; }
    }
}
