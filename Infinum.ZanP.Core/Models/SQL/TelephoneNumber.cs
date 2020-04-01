namespace Infinum.ZanP.Core.Models.SQL
{
    public class TelephoneNumber
    {
        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string AreaCode { get; set; }
        public string PhoneNumber { get; set; }

        public TelephoneNumber()
        {

        }

        public TelephoneNumber(string p_countryCode, string p_phoneNumber, string p_areaCode=null)
        {
            CountryCode = p_countryCode;
            AreaCode = p_areaCode;
            PhoneNumber = p_phoneNumber;
        }

        public override string ToString() 
        {
            return "00"+CountryCode+ AreaCode + " "+ PhoneNumber;
        }
    }
}