using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType = ResourceType.Sand;
    [SerializeField] private int gatherAmountPerInteraction = 1;
    [SerializeField] private bool permanentlyInfinite = true;

    private int currentAmount;

    public ResourceType ResourceType => resourceType;
    public int GatherAmountPerInteraction => gatherAmountPerInteraction;
    public int RemainingAmount => currentAmount;
    public bool CanGather => permanentlyInfinite || currentAmount > 0;

    private void Awake()
    {
        currentAmount = permanentlyInfinite ? int.MaxValue : 0;
    }

    public void RemoveAmount(int amount)
    {
        if (!permanentlyInfinite)
        {
            if (amount <= 0)
            {
                return;
            }

            currentAmount = Mathf.Max(0, currentAmount - amount);
        }
    }
}
