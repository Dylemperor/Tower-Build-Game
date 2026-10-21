using UnityEngine;

public class ResourceChest : MonoBehaviour
{
    [SerializeField] private Player player;

    public Player Player => player;

    private void Awake()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }

        if (player != null)
        {
            player.EnsureStorageInitialized();
        }
    }

    public void BindPlayer(Player targetPlayer)
    {
        player = targetPlayer;
        if (player != null)
        {
            player.EnsureStorageInitialized();
            player.Chest ??= new Inventory(int.MaxValue);
        }
    }

    public int DepositAllFromBackpack()
    {
        if (player == null)
        {
            return 0;
        }

        return player.DepositBackpackToChest();
    }
}
