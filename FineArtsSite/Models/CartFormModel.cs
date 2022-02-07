using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FineArtsSite.Models
{
    public class CartFormModel
    {
        public List<Item> cart { get; set; }
        public string total { get; set; }
        public string PaymentType { get; set; }
    }
}
