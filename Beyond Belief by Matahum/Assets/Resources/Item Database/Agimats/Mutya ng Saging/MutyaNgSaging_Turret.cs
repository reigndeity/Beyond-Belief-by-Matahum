using System.Collections;
using System.Linq;
using UnityEngine;

public class MutyaNgSaging_Turret : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    private string currentAnimationState = "BananaTurret_Idle";
    public ParticleSystem shootOutParticle;

    public Transform bulletSpawnPoint;
    public float bulletDamage;
    public int bulletCount;
    public int bulletsLeft;
    public GameObject bulletPrefab;
    public float range;
    public float turretDuration;

    private float startTime;
    private Vector3 dir;
    void Awake()
    {
        transform.rotation = Quaternion.identity;
    }

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
            .Where(h => h.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            .ToList();

            if (enemies.Count > 0)
            {
                // Pick the closest enemy
                Transform target = enemies
                    .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
                    .First().transform;

                // Compute direction directly toward the enemy
                dir = ((target.position) - transform.position).normalized;
            }

            float i = 0;
            float elapsed = 0.5f;
            while (i < elapsed)
            {
                i += Time.deltaTime;
                // Rotate towards player
                dir.y = 0f;
                if (dir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        50 * Time.deltaTime
                    );
                }

                yield return null;
            }

            yield return PlayAndWait("BananaTurret_Attack");
            // Wait 1 second before next attack
            yield return new WaitForSeconds(0.5f);
        }

        // After finishing bullets or duration, wait 1 sec then destroy
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void ShootBullet()
    {
        shootOutParticle.Play();
        // Spawn bullet
        GameObject bulletObj = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

        MutyaNgSaging_Bullet bullet = bulletObj.GetComponent<MutyaNgSaging_Bullet>();
        bullet.bulletDamage = bulletDamage;

        // Give the bullet its target direction
        if (dir == Vector3.zero)
            dir = transform.forward;
        bullet.SetDirection(dir);

        bulletsLeft--;
    }

    private IEnumerator PlayAndWait(string animName)
    {
        ChangeAnimationState(animName);
        yield return new WaitForSecondsRealtime(GetAnimationLength(animName));
        ChangeAnimationState("BananaTurret_Idle");
    }


    private float GetAnimationLength(string animName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animName)
                return clip.length;
        }
        return 1f;
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;
        animator.CrossFade(newState, 0.1f);
        currentAnimationState = newState;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
