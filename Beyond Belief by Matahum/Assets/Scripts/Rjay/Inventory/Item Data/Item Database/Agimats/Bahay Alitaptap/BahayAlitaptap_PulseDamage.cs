using UnityEngine;

public class BahayAlitaptap_PulseDamage : MonoBehaviour
{
    public float pulseDamage;

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(pulseDamage);
        }
    }
}
