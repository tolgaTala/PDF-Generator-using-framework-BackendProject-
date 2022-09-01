using Core.Entities;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class forPdfDto:IDto
    {
        public Customer Customer { get; set; }
        public List<Product> Products { get; set; }
        public List<List<Answers>> Answers { get; set; }
    }
}
