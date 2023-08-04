namespace WebApplication2.Models
{
    public class StoreLocation
    {
        public Guid StoreNumber { get; set; }

        public string StreetNameAndNumber { get; set; }

        private string _province;

        public string Province
        {
            get => _province;
            set
            {
                string[] validProvinces = new string[]
                {
                    "Alberta", "British Columbia", "Manitoba", "New Brunswick",
                    "Newfoundland and Labrador", "Nova Scotia", "Ontario",
                    "Prince Edward Island", "Quebec", "Saskatchewan"
                };

                if (string.IsNullOrEmpty(value) || value.Length < 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Province name must be at least three characters in length.");
                }

                if (!validProvinces.Contains(value.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Invalid Canadian province.", nameof(value));
                }

                _province = value;
            }
        }

        public HashSet<StoreLaptop> StoreLaptops { get; set; } = new HashSet<StoreLaptop>();


    }
}
