using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Customer:IEntity
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string To { get; set; }
        public string Attn { get; set; }
        public string Vessel { get; set; }
        public string DeliveryPort { get; set; }
        public string Ref { get; set; }
        public DateTime Date { get; set; }
        public string Cur { get; set; }
        public int Discount { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryCharges { get; set; }
        public int BoatServiceFee { get; set; }
        public int FreightCost { get; set; }
        public int CustomsCost { get; set; }
        public string Incoterms { get; set; }
        public List<Product> Products { get; set; }
        public int GrandTotal { get; set; }
        public string PhotoPath { get; set; }

    }
}
