using Entities.Common;

namespace Entities.Warehouseing
{
    public class WarehouseZone : BaseEntity<long>
    {
        public long WarehouseId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Capacity { get; set; }
        public int CurrentOccupancy => Inventories.Sum(i => i.Quantity);
        public int AvailableCapacity => Capacity - CurrentOccupancy;

        // Navigation properties
        public Warehouse Warehouse { get; set; }
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        // Business methods
        public bool CanStore(int quantity) => AvailableCapacity >= quantity;

        public void UpdateCapacity(int newCapacity)
        {
            if (newCapacity < CurrentOccupancy)
                throw new InvalidOperationException("New capacity cannot be less than current occupancy");

            Capacity = newCapacity;
        }
    }
}