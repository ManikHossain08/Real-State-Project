using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDLCReport.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
    }
}