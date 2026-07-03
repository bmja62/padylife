namespace Common.GridResults
{
    public class GlobalGrid
    {
        public virtual GlobalGrid DefaultPagination(int? pageNumber = 1, int? count = 10)
        {
            return new GlobalGrid
            {
                PageNumber = pageNumber,
                Count = count
            };
        }

        public string Search { get; set; }

        private int? _pageNumber;
        public int? PageNumber
        {
            get => _pageNumber ?? 1;
            set => _pageNumber = value;
        }

        private int? _count;
        public int? Count
        {
            get => (_count == null || _count <= 0) ? 10 :
                  (_count > 100) ? 100 : _count;
            set => _count = value;
        }

        public int Skip => (PageNumber.Value - 1) * Count.Value;
        public int Take => Count.Value;
    }
}