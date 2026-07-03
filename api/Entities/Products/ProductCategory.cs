using Entities.Common;

namespace Entities.Products
{
    public class ProductCategory : BaseEntity<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long? ParentCategoryId { get; set; }
        public string ImageUrl { get; set; }
        // Navigation properties
        public ProductCategory ParentCategory { get; set; }
        public ICollection<ProductCategory> ChildCategories { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<ProductCategoryAttribute> Attributes { get; set; }
    }
}
