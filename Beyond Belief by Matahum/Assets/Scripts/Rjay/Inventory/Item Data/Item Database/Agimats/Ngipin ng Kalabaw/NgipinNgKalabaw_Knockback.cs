using UnityEngine;
using System.Collections;

public class NgipinNgKalabaw_Knockback : MonoBehaviour
{
    public float damage;
    public float knockbackDistance = 5f;   // adjustable distance
    public float knockbackDuration = 0.5f; // always 0.5s but adjustable if needed

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            // Start knockback
            StartCoroutine(KnockbackEnemy(enemy.transform));
        }
    }

    private IEnumerator KnockbackEnemy(Transform enemy)
    {
        Vector3 start = enemy.position;

        // direction away from player (this GameObject's transform.position)
        Vector3 direction = (enemy.position - transform.position).normalized;
        Vector3 target = start + direction * knockbackDistance;

        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / knockbackDuration;

            // Smooth movement
            enemy.position = Vector3.Lerp(start, target, t);

            yield return null;
        }
    }
}
