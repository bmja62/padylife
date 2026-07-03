using Entities.Addresses.ECommerce.Entities;
using Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Entities.Locations
{
    public class Country : BaseEntity<long>
    {
        public long CountryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CountryName { get; set; }

        [Required]
        [MaxLength(100)]
        public string CountryNameFa { get; set; }

        [Required]
        [StringLength(2)]
        public string CountryCode { get; set; }

        [Required]
        [MaxLength(5)]
        public string PhoneCode { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Province> Provinces { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
}
