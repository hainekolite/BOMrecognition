namespace BomRainB.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User 
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public int AccountType { get; set; }

        public virtual ICollection<Revision> Revisions { get; set; }
    }
}