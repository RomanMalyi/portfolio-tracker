using System.Collections.Generic;

namespace PortfolioTracker.DataAccess.Models
{
    public class PageResult<T>
    {
        public List<T> Data { get; set; } = null!;
        public int Skip { get; set; }
        public int Take { get; set; }
        public int TotalCount { get; set; }
    }
}
