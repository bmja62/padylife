using Data.Contracts;
using Entities.Products;
using Entities.Users;
using Entities.Warehouseing;
using Microsoft.AspNetCore.Identity;
using Services.Services.StockManagerServices;

namespace Services.DataInitializer
{
    public class ProductInitializer(
     UserManager<User> userManager,
     RoleManager<Role> roleManager,
     IRepository<Warehouse> warehouseRepo,
     IRepository<Inventory> inventoryRepo,
     IRepository<Product> productRepo,
     IRepository<ProductCategory> categoryRepo,
     IRepository<ProductAttribute> attributeRepo,
     IRepository<ProductAttributeValue> attrValueRepo,
     IRepository<ProductVariant> variantRepo,
     IRepository<User> userRepo,
     IStockManagerService stockManagerService) : IDataInitializer
    {
        public void InitializeData()
        {
            if (productRepo.Table.Any())
                return;

            var userId = userRepo.Table.Select(s => s.Id).FirstOrDefault(); // فرض بر اینکه کاربری موجود است
            if (userId <= 0)
            {
                new RoleDataInitializer(roleManager).InitializeData();
                new UserDataInitializer(userManager, roleManager).InitializeData();
                new WarehouseInitializer(warehouseRepo, inventoryRepo, productRepo, variantRepo, stockManagerService).InitializeData();
                userId = userRepo.Table.Select(s => s.Id).FirstOrDefault();
            }

            // دریافت دسته‌بندی‌ها
            var mobileCategory = categoryRepo.Table.FirstOrDefault(c => c.Name == "موبایل");
            var shirtCategory = categoryRepo.Table.FirstOrDefault(c => c.Name == "پیراهن");
            var laptopCategory = categoryRepo.Table.FirstOrDefault(c => c.Name == "لپ تاپ");
            var shoesCategory = categoryRepo.Table.FirstOrDefault(c => c.Name == "کفش");
            var watchCategory = categoryRepo.Table.FirstOrDefault(c => c.Name == "ساعت");

            if (mobileCategory == null || shirtCategory == null || laptopCategory == null || shoesCategory == null || watchCategory == null)
                return;

            // افزودن 5 محصول ساده
            AddSimpleProduct(mobileCategory, userId, stockManagerService);
            AddSimpleProduct(laptopCategory, userId, stockManagerService);
            AddSimpleProduct(shoesCategory, userId, stockManagerService);
            AddSimpleProduct(watchCategory, userId, stockManagerService);
            AddSimpleProduct(shirtCategory, userId, stockManagerService);

            // افزودن 5 محصول متغیر
            AddVariantProduct(mobileCategory, userId, stockManagerService);
            AddVariantProduct(shirtCategory, userId, stockManagerService);
            AddVariantProduct(laptopCategory, userId, stockManagerService);
            AddVariantProduct(shoesCategory, userId, stockManagerService);
            AddVariantProduct(watchCategory, userId, stockManagerService);
        }

        private void AddSimpleProduct(ProductCategory category, long userId, IStockManagerService stockManagerService)
        {
            // محصول ساده
            var product = new Product
            {
                Name = $"{category.Name} مدل A1",
                Description = $"{category.Name} ساده با امکانات پایه",
                Price = 1000000,
                CategoryId = category.Id,
                CreatedByUserId = userId,
                Type = ProductType.Simple
            };
            productRepo.Add(product);

            // تنظیم موجودی اولیه برای هر محصول

            stockManagerService.IncreaseStockAsync(
                product,
                new Random().Next(5, 20), // موجودی تصادفی بین 5 تا 20
                1, // فرض می‌کنیم انبار با ID=1 وجود دارد
                "موجودی اولیه سیستم").Wait();


            // ویژگی‌های محصول ساده
            AddProductAttributeValues(product.Id, "برند", "سامسونگ");
            AddProductAttributeValues(product.Id, "وزن", "180 گرم");
            AddProductAttributeValues(product.Id, "رنگ", "سفید");

        }

        private void AddVariantProduct(ProductCategory category, long userId, IStockManagerService stockManagerService)
        {
            // محصول متغیر
            var product = new Product
            {
                Name = $"{category.Name} مدل کلاسیک",
                Description = $"{category.Name} با رنگ‌ها و سایزهای مختلف",
                Price = 1500000,
                CategoryId = category.Id,
                CreatedByUserId = userId,
                Type = ProductType.Variant
            };
            productRepo.Add(product);

            // ویژگی‌های پایه محصول متغیر
            AddProductAttributeValues(product.Id, "جنس", "چرم");

            // اضافه کردن واریانت‌ها (سایز و رنگ)
            AddProductVariants(product.Id, stockManagerService);

            productRepo.Update(product);
        }

        private void AddProductAttributeValues(long productId, string attributeName, string value)
        {
            var attribute = attributeRepo.Table.FirstOrDefault(a => a.Name == attributeName);
            if (attribute != null)
            {
                var attrValue = new ProductAttributeValue
                {
                    ProductId = productId,
                    AttributeId = attribute.Id,
                    Value = value
                };
                attrValueRepo.Add(attrValue);
            }
        }

        private void AddProductVariants(long productId, IStockManagerService stockManagerService)
        {
            var sizeAttribute = attributeRepo.Table.FirstOrDefault(a => a.Name == "سایز");
            var colorAttribute = attributeRepo.Table.FirstOrDefault(a => a.Name == "رنگ");

            if (sizeAttribute != null && colorAttribute != null)
            {
                // واریانت‌ها (سایز و رنگ‌های مختلف)
                var variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = productId,
                    SKU = $"SKU_{productId}_L_Red",
                    Price = 1600000, // قیمت برای این واریانت
                    AttributeValues = new List<VariantAttributeValue>
                    {
                        new VariantAttributeValue { AttributeId = sizeAttribute.Id, Value = "L" },
                        new VariantAttributeValue { AttributeId = colorAttribute.Id, Value = "قرمز" }
                    }
                },
                new ProductVariant
                {
                    ProductId = productId,
                    SKU = $"SKU_{productId}_M_Black",
                    Price = 1600000,
                    AttributeValues = new List<VariantAttributeValue>
                    {
                        new VariantAttributeValue { AttributeId = sizeAttribute.Id, Value = "M" },
                        new VariantAttributeValue { AttributeId = colorAttribute.Id, Value = "مشکی" }
                    }
                },
                new ProductVariant
                {
                    ProductId = productId,
                    SKU = $"SKU_{productId}_L_Black",
                    Price = 1600000,
                    AttributeValues = new List<VariantAttributeValue>
                    {
                        new VariantAttributeValue { AttributeId = sizeAttribute.Id, Value = "L" },
                        new VariantAttributeValue { AttributeId = colorAttribute.Id, Value = "مشکی" }
                    }
                },
                new ProductVariant
                {
                    ProductId = productId,
                    SKU = $"SKU_{productId}_M_White",
                    Price = 1600000,
                    AttributeValues = new List<VariantAttributeValue>
                    {
                        new VariantAttributeValue { AttributeId = sizeAttribute.Id, Value = "M" },
                        new VariantAttributeValue { AttributeId = colorAttribute.Id, Value = "سفید" }
                    }
                },
                new ProductVariant
                {
                    ProductId = productId,
                    SKU = $"SKU_{productId}_L_White",
                    Price = 1600000,
                    AttributeValues = new List<VariantAttributeValue>
                    {
                        new VariantAttributeValue { AttributeId = sizeAttribute.Id, Value = "L" },
                        new VariantAttributeValue { AttributeId = colorAttribute.Id, Value = "سفید" }
                    }
                }
            };


                variantRepo.AddRange(variants);

                // تنظیم موجودی اولیه برای هر واریانت
                foreach (var variant in variants)
                {
                    stockManagerService.IncreaseStockAsync(
                        variant,
                        new Random().Next(5, 20), // موجودی تصادفی بین 5 تا 20
                        1, // فرض می‌کنیم انبار با ID=1 وجود دارد
                        "موجودی اولیه سیستم").Wait();
                }

                variantRepo.UpdateRange(variants);
            }
        }
    }


}
