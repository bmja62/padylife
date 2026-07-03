using Entities.Locations;
using System.ComponentModel.DataAnnotations;
using WebFramework.Api;

namespace PadyLife.Api.Models.DTOs
{
    public class ProvinceDTO : BaseDto<ProvinceDTO, Province, long>
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


    }

}
