using Entities.Common;
using Entities.Warehouseing;
using System.ComponentModel.DataAnnotations;

namespace Entities.Products
{
    public class ProductVariant : BaseEntity<long>
    {
        public long ProductId { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        //public int StockQuantity { get; set; }

        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string Barcode { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public ICollection<VariantAttributeValue> AttributeValues { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; } = new HashSet<Inventory>();

        [Timestamp]
        public byte[] RowVersion { get; set; }


        // Business methods
        public string GetVariantIdentifier()
        {
            var attributes = AttributeValues?
                .OrderBy(a => a.Attribute.Name)
                .Select(a => $"{a.Attribute.Name}:{a.Value}");

            return attributes != null ? string.Join(", ", attributes) : "Base Variant";
        }
    }
}
