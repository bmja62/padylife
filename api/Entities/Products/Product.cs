using Entities.Common;
using Entities.Users;
using Entities.Warehouseing;
using System.ComponentModel.DataAnnotations;

namespace Entities.Products
{
    public class Product : BaseEntity<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public long CategoryId { get; set; }
        public long CreatedByUserId { get; set; }
        public ProductType Type { get; set; }

        // New properties for warehouse management
        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string Barcode { get; set; }
        public bool TrackInventory { get; set; } = true;

        // Navigation properties
        public ProductCategory Category { get; set; }
        public ICollection<ProductAttributeValue> AttributeValues { get; set; }
        public ICollection<ProductVariant> Variants { get; set; }
        public User CreatedByUser { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; } = new HashSet<Inventory>();

        //Versioning

        [Timestamp]
        public byte[] RowVersion { get; set; }

        // Business methods
        public bool HasVariants() => Variants != null && Variants.Any();

        public void AddVariant(ProductVariant variant)
        {
            if (variant == null)
                throw new ArgumentNullException(nameof(variant));

            if (Type != ProductType.Variant)
                throw new InvalidOperationException("Cannot add variant to simple product");

            variant.Product = this;
            Variants.Add(variant);
        }


    }
}
