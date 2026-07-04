using Sandbox;
using System.Collections.Generic;

public sealed class MinerBattery : Component
{
    [Sync] public float CurrentGas { get; set; } = 100f;
    [Sync] public float MaxGas { get; set; } = 100f;
    [Sync] public bool IsOverclocked { get; set; } = false;

    public List<MinerShelf> ConnectedShelves { get; set; } = new();
    public int MaxConnections => IsOverclocked ? 3 : 2;

    protected override void OnFixedUpdate()
    {
        if (IsProxy) return; // Only process drain on the server

        int activeShelves = ConnectedShelves.Count;
        if (activeShelves == 0 || CurrentGas <= 0) return;

        // Drain rates calculated based on maximum gas to match real-time requirements
        float drainPerSecond = 0f;
        
        if (activeShelves == 1) 
            drainPerSecond = MaxGas / 3600f; // 1 Hour
        else if (activeShelves == 2) 
            drainPerSecond = MaxGas / 2700f; // 45 Minutes
        else if (activeShelves >= 3) 
            drainPerSecond = MaxGas / 2400f; // 40 Minutes (Overclocked)

        CurrentGas -= drainPerSecond * Time.Delta;
        if (CurrentGas < 0) CurrentGas = 0;
    }

    [Rpc.Server]
    public void BuyGasRefill()
    {
        var caller = Rpc.Caller;
        if (!DXRPCryptoConfig.TryTakeMoney(caller, DXRPCryptoConfig.GasTankCost)) return;
        
        CurrentGas = MaxGas;
    }

    [Rpc.Server]
    public void BuyOverclock()
    {
        if (IsOverclocked) return;
        
        var caller = Rpc.Caller;
        if (!DXRPCryptoConfig.TryTakeMoney(caller, DXRPCryptoConfig.OverclockCost)) return;
        
        IsOverclocked = true;
    }
}
