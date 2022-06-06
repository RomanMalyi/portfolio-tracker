﻿using PortfolioTracker.Events.Common;

namespace PortfolioTracker.Domain.Models
{
    public class Asset
    {
        public string Id { get; set; } = null!;
        public string AccountId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Currency Currency { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public double Invested { get; set; }
        public double InterestRate { get; set; }
    }
}