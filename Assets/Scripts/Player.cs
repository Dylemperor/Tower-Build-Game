using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Interaction")]
    [Tooltip("Trigger collider that defines how close the player must be to interact.")]
    [SerializeField] private Collider2D interactionRange;

    [Header("Backpack")]
    [SerializeField] private int startingBackpackCapacity = 15;
    [SerializeField] private int backpackUpgradeCost = 25;
    [SerializeField] private int backpackUpgradeCostIncrease = 10;
    [SerializeField] private int backpackUpgradeIncrease = 5;
    [SerializeField] private int money = 0;

    private readonly List<Collider2D> overlapResults = new List<Collider2D>();
    private ContactFilter2D interactableFilter;

    public Inventory Backpack { get; private set; }
    public int Money => money;
    public int BackpackUpgradeCost => backpackUpgradeCost;
    public int BackpackUpgradeIncrease => backpackUpgradeIncrease;

    private void Awake()
    {
        Backpack = new Inventory(startingBackpackCapacity);

        // Resource nodes and chests use trigger colliders, so include triggers.
        interactableFilter.useTriggers = true;
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed += OnInteractPerformed;
            interactAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteractPerformed;
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        TryInteract();
    }

    /// <summary>Gathers from a nearby resource, or deposits into a nearby chest if there is no resource.</summary>
    public void TryInteract()
    {
        ResourceNode resourceNode = FindNearby<ResourceNode>();
        if (resourceNode != null)
        {
            TryGather(resourceNode);
            return;
        }

        ResourceChest chest = FindNearby<ResourceChest>();
        if (chest != null)
        {
            chest.DepositAllFrom(Backpack);
        }
    }

    public bool TryGather(ResourceNode resourceNode)
    {
        if (resourceNode == null || !resourceNode.CanGather)
        {
            return false;
        }

        int gatheredAmount = Backpack.Add(resourceNode.ResourceType, resourceNode.NextGatherAmount);
        resourceNode.RemoveAmount(gatheredAmount);
        return gatheredAmount > 0;
    }

    public bool TryUpgradeBackpack()
    {
        if (money < backpackUpgradeCost)
        {
            return false;
        }

        money -= backpackUpgradeCost;
        Backpack.IncreaseCapacity(backpackUpgradeIncrease);
        backpackUpgradeCost += backpackUpgradeCostIncrease;
        return true;
    }

    public void AddMoney(int amount)
    {
        if (amount > 0)
        {
            money += amount;
        }
    }

    // Checks what is currently inside the interaction range, so there is no
    // enter/exit bookkeeping that can get out of sync.
    private T FindNearby<T>() where T : Component
    {
        if (interactionRange == null)
        {
            Debug.LogWarning("Player has no interaction range collider assigned.", this);
            return null;
        }

        interactionRange.Overlap(interactableFilter, overlapResults);

        foreach (Collider2D overlap in overlapResults)
        {
            if (overlap.TryGetComponent(out T component))
            {
                return component;
            }
        }

        return null;
    }
}
