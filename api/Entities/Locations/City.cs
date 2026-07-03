using Entities.Addresses.ECommerce.Entities;
using Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Entities.Locations
{
    // City.cs
    public class City : BaseEntity<long>
    {


        [Required]
        public long ProvinceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CityName { get; set; }

        [Required]
        [MaxLength(100)]
        public string CityNameFa { get; set; }

        [MaxLength(10)]
        public string CityCode { get; set; }

        public bool IsActive { get; set; } = true;

        public Province Province { get; set; }

        public ICollection<Address> Addresses { get; set; }
    }
}
