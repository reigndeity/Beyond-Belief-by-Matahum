using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactName = "Interactable Item";
    public Sprite icon;
    public KeyCode interactKey = KeyCode.F;

    [HideInInspector] public bool isInRange = false;

    [Header("Interaction Cooldown")]
    public bool useInteractCooldown = false;
    public float interactCooldown = 1f;
    private bool isOnCooldown = false;

    public virtual void OnInteract()
    {
        // If cooldown is active, ignore
        if (useInteractCooldown && isOnCooldown) return;

        // If cooldown should start immediately on interaction
        if (useInteractCooldown)
            StartCoroutine(StartCooldown());

        Debug.Log("Interacted with " + interactName);
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }

    public void TriggerCooldown()
    {
        if (useInteractCooldown)
            StartCoroutine(StartCooldown());
    }

    private System.Collections.IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(interactCooldown);
        isOnCooldown = false;
    }
}
