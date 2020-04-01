namespace Infinum.ZanP.Core.Models.SQL
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; } // there can be additional letter -> eg. 76A
        public string ZIP { get; set; }
        public string City { get; set; }
        public Country Country { get; set; }

        public Address()
        {
        }

        public Address(string p_street, string p_houseNumber, string p_zip, string p_city, Country p_country)
        {
            Street = p_street;
            HouseNumber = p_houseNumber;
            ZIP = p_zip;
            City = p_city;
            Country = p_country;
        }

        public override string ToString()
        {
            return Street + " " + HouseNumber + ", " + ZIP + " " + City + " (" + Country + ")";
        }

        public override bool Equals(object? obj)
        {
            Address compared = obj as Address;

            if(compared == null)
                return false;
            
            if((Country == null) || (compared.Country == null))
                return false;

            return ((compared.City == City) && (compared.ZIP == ZIP) && (compared.Street == Street) && (compared.HouseNumber == HouseNumber)
            && (compared.Country.Id == Country.Id));
        }
    }
}