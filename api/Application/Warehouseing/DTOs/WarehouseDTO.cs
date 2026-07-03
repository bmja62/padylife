namespace Application.Warehouseing.DTOs
{
    namespace Application.Warehousing.DTOs
    {
        public class WarehouseDTO
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string Address { get; set; }
            public bool IsActive { get; internal set; }
            public decimal Latitude { get; internal set; }
            public decimal Longitude { get; internal set; }
        }

        public class WarehouseDetailDTO : WarehouseDTO
        {
            public string ContactPhone { get; set; }
            public string ManagerName { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public bool IsActive { get; set; }
            public List<WarehouseZoneDTO> Zones { get; set; }
        }

        public class WarehouseZoneDTO
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public int Capacity { get; set; }
            public int CurrentOccupancy { get; set; }
        }

        public class CreateWarehouseDTO
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Address { get; set; }
            public string ContactPhone { get; set; }
            public string ManagerName { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
        }

        public class UpdateWarehouseDTO : CreateWarehouseDTO
        {
            public bool IsActive { get; set; }
        }

        public class AddZoneDTO
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public int Capacity { get; set; }
        }
    }
}
