using UnityEngine;

public class NunoTeleportInteractable : Interactable
{
    public override void OnInteract()
    {

        Loader.Load(5);
    }
}
