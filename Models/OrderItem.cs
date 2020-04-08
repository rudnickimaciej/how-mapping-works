using System;
using System.Collections.Generic;
using System.Text;

namespace Refleksja.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderCode { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
