namespace Infinum.ZanP.Core.Models.SQL
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString() 
        {
            return Name;
        }
    }
}