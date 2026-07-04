using Sandbox;

public static class DXRPCryptoConfig
{
    // Core Settings
    public static float ScanRadius = 60f; // Approx 5 feet (1 unit = 1 inch)
    public static int MaxShelvesPerPlayer = 3;
    
    // Economy Tuning
    public static float GasTankCost = 500f;
    public static float OverclockCost = 2000f;
    public static float UpgradeMinerCost = 1000f;
    public static float BaseMineRate = 5f; // Dollars generated per second per miner
    
    // --- SERVER OWNER CONFIGURATION HOOKS ---
    
    /// <summary>
    /// Server owners: Link this to your DXRP AddMoney/AddCash function.
    /// </summary>
    public static void AddMoneyToWallet(Connection playerConnection, float amount)
    {
        // Example integration: 
        // var playerInfo = playerConnection.GameObject.Components.Get<DXRP.PlayerWallet>();
        // playerInfo.AddMoney(amount);
        Log.Info($"[DXRP Hook] Added ${amount} to {playerConnection.DisplayName}");
    }

    /// <summary>
    /// Server owners: Link this to your DXRP Job checking function.
    /// </summary>
    public static bool IsBitminerJob(Connection playerConnection)
    {
        // Example integration:
        // return playerConnection.GameObject.Components.Get<DXRP.PlayerJob>().JobName == "bitminer";
        return true; 
    }
    
    /// <summary>
    /// Server owners: Link this to your DXRP TakeMoney function to handle purchases.
    /// Returns true if they had enough money and it was taken.
    /// </summary>
    public static bool TryTakeMoney(Connection playerConnection, float amount)
    {
        // Add your DXRP logic here.
        return true; 
    }
}
