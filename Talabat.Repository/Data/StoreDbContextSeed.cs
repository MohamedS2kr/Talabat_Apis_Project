using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.Repository.Data
{
    public static class StoreDbContextSeed
    {
        public static async Task SeedAsync(StoreDbContext dbContext)
        {
            //Seeding Brand 
            if(dbContext.productBrands.Count() == 0)
            {
                //1. Read Data From Json File 
                var BrandData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/brands.json");
                //2. Convert Json String to The Needed Type
                var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandData);

                if (Brands?.Count > 0)
                {
                    foreach (var brand in Brands)
                    {
                        dbContext.Set<ProductBrand>().Add(brand);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
            //=================================================
            //seeding category
            if(dbContext.productCategories.Count() == 0) 
            {
                // 1. Reda Data From Json File
                var categoryData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/categories.json");
                //2. Convert Json string To The Needed Type
                var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoryData);

                if (categories?.Count() > 0)
                {
                    foreach (var category in categories)
                    {
                        dbContext.Set<ProductCategory>().Add(category);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
            //==============================================================
            //seeding product
            if(dbContext.Products.Count() == 0)
            {
                // 1. Reda Data From Json File
                var productData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/products.json");
                //2. Convert Json string To The Needed Type
                var Products = JsonSerializer.Deserialize<List<Product>>(productData);

                if (Products?.Count() > 0)
                {
                    foreach (var product in Products)
                    {
                        dbContext.Set<Product>().Add(product);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
            //==============================================================
            //seeding product
            if (dbContext.DeliveryMethod.Count() == 0)
            {
                // 1. Reda Data From Json File
                var deliveryData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/delivery.json");
                //2. Convert Json string To The Needed Type
                var deliveryMethod = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);

                if (deliveryMethod?.Count() > 0)
                {
                    foreach (var delivery in deliveryMethod)
                    {
                         dbContext.Set<DeliveryMethod>().Add(delivery);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
