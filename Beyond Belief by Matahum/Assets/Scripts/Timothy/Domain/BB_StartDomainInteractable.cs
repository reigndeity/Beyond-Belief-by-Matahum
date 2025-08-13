using UnityEngine;

public class BB_StartDomainInteractable : Interactable
{
    public override void OnInteract()
    {
        BB_DomainManager.instance.StartDomain();
        Destroy(gameObject);
    }
}
