using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Product:IEntity
    {
        public int ProductId { get; set; }
        public string RefNo { get; set; }
        public string PONO { get; set; }
        public int SubTotal { get; set; }
        public int AllTotal { get; set; }
        public int CustomerId { get; set; }
        public List<Answers> Answers { get; set; }
    }
}
