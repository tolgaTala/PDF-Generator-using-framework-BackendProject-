using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Answers:IEntity
    {
        [Key]
        public int AnswerId { get; set; }
        public string No { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Qnty { get; set; }
        public int Unit { get; set; }
        public int Price { get; set; }
        public int AlterNativeQnty { get; set; }
        public int AlterNativeUnit { get; set; }
        public int AlterNativePrice { get; set; }
        public int total { get; set; }
        public string Remarks { get; set; }
        public int ProductId { get; set; }
    }
}
