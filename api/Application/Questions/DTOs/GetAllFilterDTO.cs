using Common.GridResults;

namespace Application.Questions.DTOs
{
    public class GetAllFilterDTO : GlobalGrid
    {
        public string Search { get; set; }
    }
}
