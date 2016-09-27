namespace BomRainB.Models
{
    using System;

    public class Revision
    {

        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string DocuemntVersion { get; set; }
        public DateTime Date { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

    }
}