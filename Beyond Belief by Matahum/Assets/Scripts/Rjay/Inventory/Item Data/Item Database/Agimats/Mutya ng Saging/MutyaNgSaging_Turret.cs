using System.Collections;
using System.Linq;
using UnityEngine;

public class MutyaNgSaging_Turret : MonoBehaviour
{
    public float bulletDamage;
    public int bulletCount;
    public int bulletsLeft;
    public GameObject bulletPrefab;
    public float range;
    public float turretDuration;

    private float startTime;

    private void Start()
    {
        bulletsLeft = bulletCount;
        startTime = Time.time;
        StartCoroutine(AttackingEnemies());
    }

    IEnumerator AttackingEnemies()
    {
        while (bulletsLeft > 0 && Time.time - startTime < turretDuration)
        {
            // Find all enemies in range
            Collider[] hits = Physics.OverlapSphere(transform.position, range);
            var enemies = hits
                .Select(h => h.GetComponent<Enemy>())
                .Where(e => e != null)
                .ToList();

            if (enemies.Count > 0)
            {
                // Pick the closest enemy
                Transform target = enemies
                    .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
                    .First().transform;

                // Compute direction directly toward the enemy
                Vector3 offset = new Vector3(0,0.5f,0);
                Vector3 dir = ((target.position + offset) - transform.position).normalized;

                // Spawn bullet
                GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

                MutyaNgSaging_Bullet bullet = bulletObj.GetComponent<MutyaNgSaging_Bullet>();
                bullet.bulletDamage = bulletDamage;

                // Give the bullet its target direction
                bullet.SetDirection(dir);

                bulletsLeft--;
            }

            // Wait 1 second before next attack
            yield return new WaitForSeconds(1f);
        }

        // After finishing bullets or duration, wait 1 sec then destroy
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
