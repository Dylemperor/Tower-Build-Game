using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Sand,
    Dirt,
    Wood,
    Stone
}

// Plain C# class (not serialized by Unity): create it from code with a capacity.
public class Inventory
{
    private readonly Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    private int capacity;
    private int currentTotal;

    public Inventory(int capacity)
    {
        this.capacity = Mathf.Max(0, capacity);
    }

    public int Capacity => capacity;
    public int CurrentTotal => currentTotal;
    public int RemainingCapacity => Mathf.Max(0, capacity - currentTotal);
    public bool IsFull => RemainingCapacity == 0;
    public IReadOnlyDictionary<ResourceType, int> Resources => resources;

    public int GetCount(ResourceType resourceType)
    {
        return resources.TryGetValue(resourceType, out int count) ? count : 0;
    }

    public bool Contains(ResourceType resourceType, int amount = 1)
    {
        return GetCount(resourceType) >= amount;
    }

    public void IncreaseCapacity(int amount)
    {
        if (amount > 0)
        {
            capacity += amount;
        }
    }

    /// <summary>Adds as much as fits. Returns the amount actually added.</summary>
    public int Add(ResourceType resourceType, int amount)
    {
        int addedAmount = Mathf.Min(amount, RemainingCapacity);
        if (addedAmount <= 0)
        {
            return 0;
        }

        resources[resourceType] = GetCount(resourceType) + addedAmount;
        currentTotal += addedAmount;
        return addedAmount;
    }

    /// <summary>Removes up to the requested amount. Returns the amount actually removed.</summary>
    public int Remove(ResourceType resourceType, int amount)
    {
        int currentAmount = GetCount(resourceType);
        int removedAmount = Mathf.Min(amount, currentAmount);
        if (removedAmount <= 0)
        {
            return 0;
        }

        int updatedAmount = currentAmount - removedAmount;
        if (updatedAmount == 0)
        {
            resources.Remove(resourceType);
        }
        else
        {
            resources[resourceType] = updatedAmount;
        }

        currentTotal -= removedAmount;
        return removedAmount;
    }

    /// <summary>Moves everything that fits into the target. Returns the total amount moved.</summary>
    public int TransferAllTo(Inventory target)
    {
        if (target == null)
        {
            Debug.LogWarning("Inventory transfer attempted with a null target.");
            return 0;
        }

        int movedTotal = 0;

        // Copy the keys because Remove() modifies the dictionary while we loop.
        foreach (ResourceType resourceType in new List<ResourceType>(resources.Keys))
        {
            int movedAmount = target.Add(resourceType, GetCount(resourceType));
            Remove(resourceType, movedAmount);
            movedTotal += movedAmount;
        }

        return movedTotal;
    }

    public void Clear()
    {
        resources.Clear();
        currentTotal = 0;
    }
}
