using UnityEngine;

public class SaveTrigger : MonoBehaviour
{
    private bool isTriggered = false;

    private async void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return; // ✅ prevent multiple triggers

        if (other.CompareTag("Player"))
        {
            isTriggered = true;

            // Wait for save to finish
            await GameManager.instance.SaveAll();

            // Destroy trigger after save completes
            this.gameObject.SetActive(false);
        }
    }
}
