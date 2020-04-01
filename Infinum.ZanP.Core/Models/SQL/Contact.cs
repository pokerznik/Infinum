using System;
using System.Collections.Generic;

namespace Infinum.ZanP.Core.Models.SQL
{
    public class Contact 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Address Address { get; set; }
        public IEnumerable<TelephoneNumber> TelephoneNumbers { get; set; }
    }
}