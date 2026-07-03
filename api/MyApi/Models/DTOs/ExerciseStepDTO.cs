using AutoMapper;
using Entities.Excersies;
using WebFramework.Api;

namespace PadyLife.Api.Models.DTOs
{
    public class StepDTO : BaseDto<StepDTO, Step, long>
    {
        public string Name { get; set; }
        public long? CreatedByUserId { get; set; }
        public List<StepOptionDTO> StepOptions { get; set; } 
        public override void CustomMappings(IMappingExpression<Step, StepDTO> mapping)
        {
            mapping.ForMember(
                dest => dest.StepOptions,
                config => config.MapFrom(src => src.StepOptions.Select(so => new StepOptionDTO
                {
                    Id = so.Id,
                    StepId = so.StepId,
                    Title = so.Title,
                })));
        }
    }
    public class StepOptionDTO
    {
        public long Id { get;  set; }
        public long StepId { get;  set; }
        public string Title { get;  set; }
    }
}
