using Microsoft.EntityFrameworkCore;
using System.Net;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public static class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)  //the Initialize method initializes the database
        {
            LaptopStoreContext db = new LaptopStoreContext(serviceProvider.GetRequiredService<DbContextOptions<LaptopStoreContext>>());

            db.Database.EnsureDeleted();//Be cautious when using this in a production environment, as it will delete all data in the database.
            db.Database.Migrate();


            //brand
            Brand brand1 = new Brand { Id = Guid.NewGuid(), Name = "Dell" };
            Brand brand2 = new Brand { Id = Guid.NewGuid(), Name = "Surface" };
            Brand brand3 = new Brand { Id = Guid.NewGuid(), Name = "Apple" };

            if (!db.Brands.Any())
            {
                db.Add(brand1);
                db.Add(brand2);
                db.Add(brand3);
                db.SaveChanges();
            }

            // Laptop
            Laptop laptop1 = new Laptop { Model = "Model 1", Brand =brand1 , Price = 1000, Condition = LaptopCondition.New };
            Laptop laptop2 = new Laptop { Model = "Model 2", Brand = brand2, Price = 1200, Condition = LaptopCondition.New };
            Laptop laptop3 = new Laptop { Model = "Model 3", Brand = brand3, Price = 800 , Condition = LaptopCondition.New };

            laptop1.Brand = brand1;
            laptop2.Brand = brand2;
            laptop3.Brand = brand3;
           

            if (!db.Laptops.Any())
            {
                db.Add(laptop1);
                db.Add(laptop2);
                db.Add(laptop3);
                db.SaveChanges();
            }

            StoreLocation store1 = new StoreLocation { StoreNumber = Guid.NewGuid(), StreetNameAndNumber = "Street 1", Province = "Ontario" };
            StoreLocation store2 = new StoreLocation { StoreNumber = Guid.NewGuid(), StreetNameAndNumber = "Street 2", Province = "British Columbia" };
            StoreLocation store3 = new StoreLocation { StoreNumber = Guid.NewGuid(), StreetNameAndNumber = "Street 3", Province = "Alberta" };


            if (!db.StoreLocations.Any())
            {
                db.Add(store1);
                db.Add(store2);
                db.Add(store3);
                db.SaveChanges();
            }

            StoreLaptop storeLaptop1 = new StoreLaptop { Laptop = laptop1, StoreLocation = store1, Quantity = 5 };
            StoreLaptop storeLaptop2 = new StoreLaptop { Laptop = laptop2, StoreLocation = store2, Quantity = 2 };
            StoreLaptop storeLaptop3 = new StoreLaptop { Laptop = laptop3, StoreLocation = store3, Quantity = 3 };

            

            if (!db.StoreLaptops.Any())
            {
                db.Add(storeLaptop1);
                db.Add(storeLaptop2);
                db.Add(storeLaptop3);
                db.SaveChanges();
            }
        }
    }
}
