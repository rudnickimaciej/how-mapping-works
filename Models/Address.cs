using Refleksja.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Refleksja.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public int HomeNo { get; set; }
        public City City { get; set; }
    }
}
