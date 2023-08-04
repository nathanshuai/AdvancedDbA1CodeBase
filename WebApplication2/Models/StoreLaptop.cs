namespace WebApplication2.Models
{
    public class StoreLaptop
    {
        
        public Guid StoreNumber { get; set; }

        public StoreLocation StoreLocation { get; set; }
        public Guid LaptopId { get; set; }

        public Laptop Laptop { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
            }
        }

    }

}
