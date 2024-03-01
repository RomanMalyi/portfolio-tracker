using System;

namespace PortfolioTracker.DataAccess.Models
{
    public class SnapshotGenerationTrigger
    {
        public string UserId { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
