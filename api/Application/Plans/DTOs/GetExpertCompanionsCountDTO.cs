
namespace Application.Plans.DTOs
{
    public class GetExpertCompanionsCountDTO
    {
        public GetExpertCompanionsCountDTO(int count)
        {
            Count = count;
        }

        public int Count { get; set; }
        internal static GetExpertCompanionsCountDTO Init(int count) => new(count);
    }
}
