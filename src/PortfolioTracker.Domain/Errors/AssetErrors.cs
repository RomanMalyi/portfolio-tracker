namespace PortfolioTracker.Domain.Errors
{
    public static class AssetErrors
    {
        public static Error AssetAlreadyCreated => new Error(nameof(AssetAlreadyCreated), "The asset is already created.");
        public static Error AssetNotCreated => new Error(nameof(AssetNotCreated), "Asset is not created.");
    }
}
