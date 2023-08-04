namespace WebApplication2.Models
{
    public class StoreByProvince
    {

        public string Province { get; set; }
        public HashSet<StoreLocation> StoreLocations { get; set; }

    }
}
