using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType = ResourceType.Sand;
    [SerializeField] private int gatherAmountPerInteraction = 1;
    [SerializeField] private bool permanentlyInfinite = true;

    [Tooltip("How much this node holds. Ignored when the node is infinite.")]
    [SerializeField] private int startingAmount = 10;

    private int remainingAmount;

    public ResourceType ResourceType => resourceType;
    public bool IsInfinite => permanentlyInfinite;
    public int RemainingAmount => remainingAmount;

    /// <summary>How much the next gather will yield (never more than the node has left).</summary>
    public int NextGatherAmount => permanentlyInfinite
        ? gatherAmountPerInteraction
        : Mathf.Min(gatherAmountPerInteraction, remainingAmount);

    public bool CanGather => NextGatherAmount > 0;

    private void Awake()
    {
        remainingAmount = Mathf.Max(0, startingAmount);
    }

    public void RemoveAmount(int amount)
    {
        if (permanentlyInfinite || amount <= 0)
        {
            return;
        }

        remainingAmount = Mathf.Max(0, remainingAmount - amount);
    }
}
