using UnityEngine;

public class Shopkeep_Interactable : Interactable
{
    public override void OnInteract()
    {
        Shopkeep shopkeep = GetComponent<Shopkeep>();
        shopkeep.shopkeeperUI.SetActive(true);
        shopkeep.OnOpenShopDetails(R_ItemType.Consumable);

        PlayerCamera playerCam = FindFirstObjectByType<PlayerCamera>();
        playerCam.SetCursorVisibility(true);
    }
}
