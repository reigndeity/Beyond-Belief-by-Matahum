using UnityEngine;

public class NeedleScript : MonoBehaviour
{
    [HideInInspector] public Vector3 lastPlayerPosition;
    public float damagePercentage = 50;
    [HideInInspector] private float damage;
    [HideInInspector] public float speed;
    [HideInInspector] public bool canShoot;
    public GameObject particleFX;

    private EnemyStats stats;
    //public GameObject explosionVFX;

    // Update is called once per frame

    private void Start()
    {
        stats = FindFirstObjectByType<Mangkukulam>().GetComponent<EnemyStats>();
        damage = (stats.e_attack * (damagePercentage / 100));
        Destroy(gameObject, 10f);
    }
    void Update()
    {
        if (canShoot)
        {
            transform.position += lastPlayerPosition * speed * Time.deltaTime;
            particleFX.SetActive(true);
        }
        else if (!canShoot)
        {
            Vector3 direction = FindFirstObjectByType<Player>().transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction.normalized);
            particleFX.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
            other.gameObject.layer == LayerMask.NameToLayer("Terrain") ||
            other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
