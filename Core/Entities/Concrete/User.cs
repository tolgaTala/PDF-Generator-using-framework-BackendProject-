using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Concrete
{
    public class User : IEntity
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        public string ImagePath { get; set; }
        public string TaxNo { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Status { get; set; }
    }
}
