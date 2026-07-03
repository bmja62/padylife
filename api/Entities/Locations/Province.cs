using Entities.Addresses.ECommerce.Entities;
using Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Entities.Locations
{
    public class Province : BaseEntity<long>
    {

        [Required]
        public long CountryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProvinceName { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProvinceNameFa { get; set; }

        [MaxLength(10)]
        public string ProvinceCode { get; set; }

        public bool IsActive { get; set; } = true;

        public Country Country { get; set; }
        public ICollection<City> Cities { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
}
