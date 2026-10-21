using UnityEngine;

public class ResourceChest : MonoBehaviour
{
    // Chest storage is unlimited.
    public Inventory Storage { get; } = new Inventory(int.MaxValue);

    /// <summary>Moves everything from the given inventory into this chest. Returns the amount moved.</summary>
    public int DepositAllFrom(Inventory source)
    {
        return source != null ? source.TransferAllTo(Storage) : 0;
    }
}
