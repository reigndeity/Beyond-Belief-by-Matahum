using System.Collections;
using System.Linq;
using UnityEngine;

public class MutyaNgSaging_Turret : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    private string currentAnimationState = "BananaTurret_Idle";
    public ParticleSystem shootOutParticle;

    [Header("Turret Settings")]
    public Transform bulletSpawnPoint;
    public float bulletDamage;
    public int bulletCount;
    public int bulletsLeft;
    public GameObject bulletPrefab;
    public float range;
    public float turretDuration;

    private float startTime;
    private Vector3 dir;
    private bool hasTarget = false;

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

            // ? Only attack if there are enemies
            if (enemies.Count > 0)
            {
                hasTarget = true;

                // Pick the closest enemy
                Transform target = enemies
                    .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
                    .First().transform;

                // Compute direction toward the target
                dir = (target.position - transform.position).normalized;

                // Rotate smoothly toward the target
                float i = 0f;
                float elapsed = 0.3f;
                while (i < elapsed)
                {
                    i += Time.deltaTime;
                    Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);
                    if (flatDir != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(flatDir);
                        transform.rotation = Quaternion.Slerp(
                            transform.rotation,
                            targetRotation,
                            10 * Time.deltaTime
                        );
                    }
                    yield return null;
                }

                // Play attack animation & shoot
                yield return PlayAndWait("BananaTurret_Attack");

                // Small delay between attacks
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                // ? No enemies — idle, don't shoot
                if (hasTarget)
                {
                    ChangeAnimationState("BananaTurret_Idle");
                    hasTarget = false;
                }

                // Wait before checking again to save performance
                yield return new WaitForSeconds(0.2f);
            }
        }

        // After duration or bullets, wait 1 sec then destroy
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void ShootBullet()
    {
        if (!hasTarget) return; // ? Do not shoot if no target

        shootOutParticle.Play();

        // Spawn bullet
        GameObject bulletObj = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        MutyaNgSaging_Bullet bullet = bulletObj.GetComponent<MutyaNgSaging_Bullet>();
        bullet.bulletDamage = bulletDamage;

        // Set direction
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
