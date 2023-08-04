using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Models.ResponseModel;
using StoreLocation = WebApplication2.Models.StoreLocation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LaptopStoreContext>(options => //The AddDbContext method registers a service in the dependency injection containe
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LaptopStoreConnectionString"));
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => { //sets up JSON serialization options to handle circular references.
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())  //It's used to ensure that any resources created within the scope are properly disposed of when the scope is exited.
{
    IServiceProvider serviceProvider = scope.ServiceProvider; //using dependency injection to obtain an instance of the IServiceProvider is a container 

    await SeedData.Initialize(serviceProvider);//seeding the initial data into the database
}
app.Run();

//No1
app.MapGet("/laptops/search", (LaptopStoreContext db, decimal ? priceAbove, decimal ? priceBelow, Guid? storeNumber, string province, 
    LaptopCondition? condition, Guid? brandId ,string searchPhrase) =>
{
    HashSet<Laptop> laptops = new HashSet<Laptop>();
    try
    {
        if (priceAbove < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(priceAbove));
        }

        if (priceBelow < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(priceBelow));
        }

        if (priceAbove.HasValue)
        {
            laptops = db.Laptops.Where(l => l.Price > priceAbove.Value).ToHashSet();
        }

        if (priceBelow.HasValue)
        {
            laptops = db.Laptops.Where(l => l.Price < priceBelow.Value).ToHashSet();
        }

        if (storeNumber.HasValue)
        {
            laptops = db.Laptops.Where(l => l.StoreLaptops.Any(sl => sl.StoreNumber == storeNumber.Value && sl.Quantity > 0)).ToHashSet();
        }

        if (!string.IsNullOrEmpty(province))
        {
            laptops = db.Laptops.Where(l => l.StoreLaptops.Any(sl => sl.StoreLocation.Province == province && sl.Quantity > 0)).ToHashSet();
        }

        if (condition.HasValue)
        {
            laptops = db.Laptops.Where(l => l.Condition == condition.Value).ToHashSet();
        }

        if (brandId.HasValue)
        {
            laptops = db.Laptops.Where(l => l.BrandId == brandId.Value).ToHashSet();
        }

        if (!string.IsNullOrEmpty(searchPhrase))
        {
            laptops = db.Laptops.Where(l => l.Model.Contains(searchPhrase)).ToHashSet();
        }

        return Results.Ok(laptops);

    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }

});

//No2
app.MapGet("stores/{storeNumber}/inventory", (LaptopStoreContext db, Guid storeNumber) =>
{
    HashSet<Laptop> Laptops = new HashSet<Laptop>();

    try
    {
        if (Guid.Empty.Equals(storeNumber))
        {
            throw new ArgumentOutOfRangeException(nameof(storeNumber));
        }

        Laptops = (HashSet<Laptop>)db.StoreLaptops.Where(sl => sl.StoreNumber == storeNumber && sl.Quantity > 0)
                     .Select(sl => new
                     {
                         LaptopId = sl.LaptopId,
                         LaptopModel = sl.Laptop.Model,
                         LaptopBrand = sl.Laptop.Brand,
                         Quantity = sl.Quantity
                     });

        return Results.Ok(Laptops);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }



});

//No3
app.MapPost("/stores/{storeNumber}/{laptopNumber}/changeQuantity", (LaptopStoreContext db, Guid storeNumber, Guid laptopNumber, int amount) =>
{
    try
    {

        StoreLaptop storeLaptop = db.StoreLaptops.First(sl => sl.StoreNumber == storeNumber && sl.LaptopId == laptopNumber);

        if (storeLaptop == null)
        {
            return Results.NotFound("Store laptop not found.");
        }

        if (amount < 0)
        {
            return Results.BadRequest("Amount must be a positive value.");
        }

        storeLaptop.Quantity = amount;
        db.SaveChanges();

    
        return Results.Ok(storeLaptop);

    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

//No4 An endpoint for getting the average price of all laptops among a specific brand,
//returned as { LaptopCount: [value], AveragePrice: [value]}

app.MapGet("/brands/{brandId}/averagePrice", (LaptopStoreContext db, Guid brandId) =>
{
   
    try
    {
        if (Guid.Empty.Equals(brandId))
        {
            throw new ArgumentOutOfRangeException(nameof(brandId));
        }

        HashSet<Laptop> brandLaptops = db.Laptops
                    .Where(l => l.BrandId == brandId).ToHashSet();

        int laptopCount = brandLaptops.Count();

        if (laptopCount == 0)
        {
            return Results.NotFound("No laptops found for the specified brand.");
        }

        decimal averagePrice = brandLaptops.Average(l => l.Price);

        BrandAveragePriceResult result = new BrandAveragePriceResult
        {
            LaptopCount = laptopCount,
            AveragePrice = averagePrice
        };

        return Results.Ok(result);

    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }

});

//No5 An endpoint which dynamically groups and returns all Stores according to the Province
//in which they are in. This endpoint should not display any data from any model other than the
//Stores queried, and should only display provinces that have stores in them.

app.MapGet("/stores/groupedByProvince", (LaptopStoreContext db) =>
{

    try
    {
        HashSet<StoreByProvince> storesByProvince = db.StoreLocations
                .GroupBy(sl => sl.Province)
                .Where(group => group.Any()) // filters out empty groups 
                    .Select(group => new StoreByProvince
                    {
                        Province = group.Key,
                        StoreLocations = new HashSet<StoreLocation>(
                        group.Select(s => new StoreLocation //populating with StoreLocation objects for each store in a specific province
                        {
                            StoreNumber = s.StoreNumber,
                            StreetNameAndNumber = s.StreetNameAndNumber
                        }))
                    }).ToHashSet();

        return Results.Ok(storesByProvince);

    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    }

});
