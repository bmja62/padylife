using Entities.Common;

namespace Entities.Baskets
{
    public class BasketItem : BaseEntity<long>
    {
        public long BasketId { get; set; }
        public long ObjectId { get; set; }
        public BasketItemType ItemType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Basket Basket { get; set; }
    }



}
