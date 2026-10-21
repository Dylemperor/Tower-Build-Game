using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Backpack")]
    [SerializeField] private int startingBackpackCapacity = 15;
    [SerializeField] private int backpackUpgradeCost = 25;
    [SerializeField] private int backpackUpgradeIncrease = 5;
    [SerializeField] private int money = 0;

    [Header("Storage")]
    [SerializeField] private Inventory backpack;
    [SerializeField] private Inventory chest;

    private ResourceNode nearbyResource;
    private ResourceChest nearbyChest;

    public Inventory Backpack { get => backpack; set => backpack = value ?? new Inventory(startingBackpackCapacity); }
    public Inventory Chest { get => chest; set => chest = value ?? new Inventory(int.MaxValue); }
    public int Money => money;
    public int BackpackUpgradeCost => backpackUpgradeCost;
    public int BackpackUpgradeIncrease => backpackUpgradeIncrease;
    public ResourceNode NearbyResource => nearbyResource;
    public ResourceChest NearbyChest => nearbyChest;

    private void Awake()
    {
        if (backpack == null)
        {
            backpack = new Inventory(startingBackpackCapacity);
        }

        if (chest == null)
        {
            chest = new Inventory(int.MaxValue);
        }
    }

    public void EnsureStorageInitialized()
    {
        backpack ??= new Inventory(startingBackpackCapacity);
        chest ??= new Inventory(int.MaxValue);
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed += OnInteractPerformed;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        ResourceNode resourceNode = other.GetComponent<ResourceNode>();
        if (resourceNode != null)
        {
            nearbyResource = resourceNode;
            return;
        }

        ResourceChest chestObject = other.GetComponent<ResourceChest>();
        if (chestObject != null)
        {
            nearbyChest = chestObject;
            chestObject.BindPlayer(this);
            EnsureStorageInitialized();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ResourceNode resourceNode = other.GetComponent<ResourceNode>();
        if (resourceNode != null && nearbyResource == resourceNode)
        {
            nearbyResource = null;
        }

        ResourceChest chestObject = other.GetComponent<ResourceChest>();
        if (chestObject != null && nearbyChest == chestObject)
        {
            nearbyChest = null;
        }
    }

    public void TryInteract()
    {
        EnsureStorageInitialized();

        if (nearbyResource != null)
        {
            TryGather(nearbyResource);
            return;
        }

        if (nearbyChest != null)
        {
            DepositBackpackToChest();
        }
    }

    public bool TryGather(ResourceNode resourceNode)
    {
        EnsureStorageInitialized();

        if (resourceNode == null || backpack == null || !resourceNode.CanGather)
        {
            return false;
        }

        int gatheredAmount = backpack.Add(resourceNode.ResourceType, resourceNode.GatherAmountPerInteraction);
        if (gatheredAmount <= 0)
        {
            return false;
        }

        resourceNode.RemoveAmount(gatheredAmount);
        return true;
    }

    public int DepositBackpackToChest()
    {
        EnsureStorageInitialized();

        if (backpack == null || chest == null)
        {
            Debug.LogWarning("Player cannot deposit because backpack or chest inventory is null.");
            return 0;
        }

        if (nearbyChest != null)
        {
            nearbyChest.BindPlayer(this);
        }

        return backpack.TransferAllTo(chest);
    }

    public bool TryUpgradeBackpack()
    {
        if (money < backpackUpgradeCost)
        {
            return false;
        }

        money -= backpackUpgradeCost;
        backpack.IncreaseCapacity(backpackUpgradeIncrease);
        backpackUpgradeCost += 10;
        return true;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        money += amount;
    }
}
