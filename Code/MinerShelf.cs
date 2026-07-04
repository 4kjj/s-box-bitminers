using Sandbox;
using System.Linq;

public sealed class MinerShelf : Component
{
    [Sync] public float MinedMoney { get; set; } = 0f;
    [Sync] public int MinerCount { get; set; } = 1;
    [Sync] public bool IsLocked { get; set; } = false;
    [Sync] public string PinCode { get; set; } = "";
    
    [Sync] public MinerBattery ConnectedBattery { get; set; }
    
    // Optional: Reference to UI so we can toggle it on/off
    [Property] public GameObject ScreenObject { get; set; }

    protected override void OnFixedUpdate()
    {
        if (IsProxy) return; // Server authority only

        HandlePowerConnection();

        // Mine money if powered
        if (ConnectedBattery != null && ConnectedBattery.CurrentGas > 0)
        {
            MinedMoney += (DXRPCryptoConfig.BaseMineRate * MinerCount) * Time.Delta;
        }
    }

    private void HandlePowerConnection()
    {
        // Disconnect if battery dies or gets moved too far away
        if (ConnectedBattery != null)
        {
            float distance = Vector3.DistanceBetween(Transform.Position, ConnectedBattery.Transform.Position);
            if (ConnectedBattery.CurrentGas <= 0 || distance > DXRPCryptoConfig.ScanRadius)
            {
                ConnectedBattery.ConnectedShelves.Remove(this);
                ConnectedBattery = null;
            }
            return;
        }

        // Scan for nearest battery within 5 feet (60 units) that has room
        var batteries = Scene.GetAllComponents<MinerBattery>()
            .Where(b => Vector3.DistanceBetween(Transform.Position, b.Transform.Position) <= DXRPCryptoConfig.ScanRadius)
            .Where(b => b.ConnectedShelves.Count < b.MaxConnections)
            .OrderBy(b => Vector3.DistanceBetween(Transform.Position, b.Transform.Position));

        var closest = batteries.FirstOrDefault();
        if (closest != null)
        {
            ConnectedBattery = closest;
            closest.ConnectedShelves.Add(this);
        }
    }

    [Rpc.Server]
    public void WithdrawMoney()
    {
        var caller = Rpc.Caller;
        if (MinedMoney <= 0) return;

        DXRPCryptoConfig.AddMoneyToWallet(caller, MinedMoney);
        MinedMoney = 0f;
    }

    [Rpc.Server]
    public void BuyMinerUpgrade()
    {
        var caller = Rpc.Caller;
        if (!DXRPCryptoConfig.TryTakeMoney(caller, DXRPCryptoConfig.UpgradeMinerCost)) return;
        
        MinerCount++;
    }

    [Rpc.Server]
    public void ToggleLock(string pin)
    {
        IsLocked = !IsLocked;
        PinCode = IsLocked ? pin : "";
    }
}
