using Entities.Locations;
using System.ComponentModel.DataAnnotations;
using WebFramework.Api;

namespace PadyLife.Api.Models.DTOs
{
    public class CountryDTO : BaseDto<CountryDTO, Country, long>
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
    }

}
