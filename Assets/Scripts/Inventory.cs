using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Sand,
    Dirt,
    Wood,
    Stone
}

[System.Serializable]
public class Inventory
{
    private readonly Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    private int capacity;

    public Inventory(int startingCapacity)
    {
        capacity = Mathf.Max(0, startingCapacity);
    }

    public int Capacity => capacity;
    public int RemainingCapacity => Mathf.Max(0, capacity - CurrentTotal);
    public int CurrentTotal
    {
        get
        {
            int total = 0;
            foreach (var entry in resources)
            {
                total += entry.Value;
            }
            return total;
        }
    }

    public bool IsFull => RemainingCapacity <= 0;

    public int GetCount(ResourceType resourceType)
    {
        if (resources.TryGetValue(resourceType, out int count))
        {
            return count;
        }

        return 0;
    }

    public IReadOnlyDictionary<ResourceType, int> GetAllResources()
    {
        return resources;
    }

    public void IncreaseCapacity(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        capacity += amount;
    }

    public int Add(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
        {
            return 0;
        }

        int acceptedAmount = Mathf.Min(amount, RemainingCapacity);
        if (acceptedAmount <= 0)
        {
            return 0;
        }

        resources[resourceType] = GetCount(resourceType) + acceptedAmount;
        return acceptedAmount;
    }

    public int Remove(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
        {
            return 0;
        }

        int currentAmount = GetCount(resourceType);
        int removedAmount = Mathf.Min(amount, currentAmount);
        if (removedAmount <= 0)
        {
            return 0;
        }

        int updatedAmount = currentAmount - removedAmount;
        if (updatedAmount <= 0)
        {
            resources.Remove(resourceType);
        }
        else
        {
            resources[resourceType] = updatedAmount;
        }

        return removedAmount;
    }

    public int TransferAllTo(Inventory target)
    {
        if (target == null)
        {
            Debug.LogWarning("Inventory transfer attempted with a null target.");
            return 0;
        }

        int movedTotal = 0;
        List<ResourceType> resourceTypes = new List<ResourceType>(resources.Keys);

        foreach (ResourceType resourceType in resourceTypes)
        {
            int availableAmount = GetCount(resourceType);
            if (availableAmount <= 0)
            {
                continue;
            }

            int movedAmount = target.Add(resourceType, availableAmount);
            if (movedAmount <= 0)
            {
                continue;
            }

            Remove(resourceType, movedAmount);
            movedTotal += movedAmount;
        }

        return movedTotal;
    }

    public void Clear()
    {
        resources.Clear();
    }

    public bool Contains(ResourceType resourceType, int amount = 1)
    {
        return GetCount(resourceType) >= amount;
    }
}
