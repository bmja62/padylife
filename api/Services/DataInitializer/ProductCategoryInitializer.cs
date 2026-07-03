using Data.Contracts;
using Entities.Products;
using Services.DataInitializer;

internal class ProductCategoryInitializer(
    IRepository<ProductCategory> categoryRepo,
    IRepository<ProductCategoryAttribute> categoryAttrRepo,
    IRepository<ProductAttribute> attributeRepo) : IDataInitializer
{
    public void InitializeData()
    {
        if (categoryRepo.Table.Any())
            return;

        // سطوح بالا
        var electronics = new ProductCategory { Name = "الکترونیک", Description = "کالاهای دیجیتال" };
        var clothing = new ProductCategory { Name = "پوشاک", Description = "انواع لباس و پوشش" };
        var shoesCategory = new ProductCategory { Name = "کفش", Description = "انواع کفش" };
        var watchCategory = new ProductCategory { Name = "ساعت", Description = "انواع ساعت" };

        // سطح دوم
        var mobile = new ProductCategory { Name = "موبایل", Description = "گوشی هوشمند", ParentCategory = electronics };
        var laptop = new ProductCategory { Name = "لپ‌تاپ", Description = "رایانه‌های قابل حمل", ParentCategory = electronics };

        var men = new ProductCategory { Name = "مردانه", Description = "پوشاک مردانه", ParentCategory = clothing };
        var women = new ProductCategory { Name = "زنانه", Description = "پوشاک زنانه", ParentCategory = clothing };

        // سطح سوم
        var mobileAccessories = new ProductCategory { Name = "لوازم جانبی موبایل", ParentCategory = mobile };
        var laptopAccessories = new ProductCategory { Name = "لوازم جانبی لپ‌تاپ", ParentCategory = laptop };

        var shirt = new ProductCategory { Name = "پیراهن", ParentCategory = men };
        var pants = new ProductCategory { Name = "شلوار", ParentCategory = men };

        var manteau = new ProductCategory { Name = "مانتو", ParentCategory = women };
        var skirt = new ProductCategory { Name = "دامن", ParentCategory = women };

        var all = new List<ProductCategory>
        {
            electronics, clothing, shoesCategory,watchCategory,
            mobile, laptop,
            mobileAccessories, laptopAccessories,
            men, women,
            shirt, pants,
            manteau, skirt
        };

        categoryRepo.AddRange(all);


        // بارگذاری Attribute‌ها
        var attributes = attributeRepo.Table.ToList();
        var catAttrs = new List<ProductCategoryAttribute>();

        void Assign(ProductCategory cat, string attrName, bool isRequired = false, bool isVariant = false)
        {
            var attr = attributes.FirstOrDefault(x => x.Name == attrName);
            if (attr != null)
            {
                catAttrs.Add(new ProductCategoryAttribute
                {
                    CategoryId = cat.Id,
                    AttributeId = attr.Id,
                    IsRequired = isRequired,
                    IsVariant = isVariant
                });
            }
        }

        // نمونه‌ای از نگاشت ویژگی‌ها به دسته‌بندی‌ها
        Assign(mobile, "برند", true);
        Assign(mobile, "وزن");
        Assign(mobile, "قرمز");

        Assign(mobileAccessories, "کاتالوگ");
        Assign(laptopAccessories, "وزن");

        Assign(shirt, "سایز M", true, true);
        Assign(shirt, "قرمز", false, true);
        Assign(pants, "سایز M", true, true);

        Assign(manteau, "سایز M", true, true);
        Assign(skirt, "سایز M");

        categoryAttrRepo.AddRange(catAttrs);

    }
}
