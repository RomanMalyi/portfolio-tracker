using Newtonsoft.Json;
using PortfolioTracker.Domain.Models;
using PortfolioTracker.Events.Common;
using System;
using System.Collections.Generic;

namespace PortfolioTracker.DataAccess.Models
{
    public class Snapshot
    {
        [JsonProperty("id")]
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public DateTimeOffset GenerationTime { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CurrencyAnalytics> CurrencyAnalytics { get; set; }
        public List<AccountAnalytics> AccountAnalytics { get; set; }
        public List<AssetTypeAnalytics> AssetTypeAnalytics { get; set; }
        public List<RiskLevelAnalytics> RiskLevelAnalytics { get; set; }
        public List<TransactionTypeAnalytics> TransactionTypeAnalytics { get; set; }
    }

    public class CurrencyAnalytics
    {
        public Currency Currency { get; set; }
        public decimal PortfolioAmount { get; set; }
        public float PortfolioPercent { get; set; }
    }

    public class AccountAnalytics
    {
        //TODO: think do I need to handle deleted accounts
        public string AccountId { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public AccountType AccountType { get; set; }
        public decimal PortfolioAmount { get; set; }
        public float PortfolioPercent { get; set; }
    }

    public class AssetTypeAnalytics
    {
        public AssetType AssetType { get; set; }
        public decimal PortfolioAmount { get; set; }
        public float PortfolioPercent { get; set; }
    }

    public class RiskLevelAnalytics
    {
        public RiskLevel RiskLevel { get; set; }
        public decimal PortfolioAmount { get; set; }
        public float PortfolioPercent { get; set; }
    }

    public class TransactionTypeAnalytics
    {
        public TransactionType TransactionType { get; set; }
        public decimal PortfolioAmount { get; set; }
        public float PortfolioPercent { get; set; }
    }
}
