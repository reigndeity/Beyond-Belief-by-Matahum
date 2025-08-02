using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactName = "Interactable Item";
    public Sprite icon;
    public KeyCode interactKey = KeyCode.F;

    [HideInInspector] public bool isInRange = false;

    public virtual void OnInteract()
    {
        Debug.Log("Interacted with " + interactName);
    }
}
