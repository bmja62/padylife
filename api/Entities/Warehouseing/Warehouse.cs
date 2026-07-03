using Entities.Common;

namespace Entities.Warehouseing
{
    public class Warehouse : BaseEntity<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public string ContactPhone { get; set; }
        public string ManagerName { get; set; }
        public bool IsActive { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        // Navigation properties
        public ICollection<WarehouseZone> Zones { get; set; } = new List<WarehouseZone>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<InventoryTransaction> OutgoingTransactions { get; set; } = new List<InventoryTransaction>();
        public ICollection<InventoryTransaction> IncomingTransactions { get; set; } = new List<InventoryTransaction>();

        // Business methods
        public WarehouseZone AddZone(string name, string code, int capacity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Zone name cannot be empty");

            var zone = new WarehouseZone
            {
                WarehouseId = Id,
                Name = name,
                Code = code,
                Capacity = capacity
            };

            Zones.Add(zone);
            return zone;
        }

        public bool HasZone(long zoneId) => Zones.Any(z => z.Id == zoneId);

        public void Deactivate()
        {
            if (Inventories.Any(i => i.Quantity > 0))
                throw new InvalidOperationException("Cannot deactivate warehouse with inventory");

            IsActive = false;
        }
    }
}