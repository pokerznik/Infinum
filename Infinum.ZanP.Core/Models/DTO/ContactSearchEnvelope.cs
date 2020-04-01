using System.Collections.Generic;

namespace Infinum.ZanP.Core.Models.DTO
{
    public class ContactSearchEnvelope
    {
        public IEnumerable<Contact> Data { get; set; }
        public int FilteredNumber { get; set; }
        public int TotalNumber { get; set; }
    }
}