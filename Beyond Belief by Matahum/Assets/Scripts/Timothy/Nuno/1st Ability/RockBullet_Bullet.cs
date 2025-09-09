using UnityEngine;
using System.Collections;

public class RockBullet_Bullet : MonoBehaviour
{
    public Vector3 lastPlayerPosition;
    public float bulletDamagePercentage = 50;
    public float bulletDamage;
    public float bulletSpeed;
    public bool canShoot;

    private EnemyStats stats;
    public GameObject explosionVFX;

    // Update is called once per frame

    private void Start()
    {
        stats = FindFirstObjectByType<Nuno>().GetComponent<EnemyStats>();
        bulletDamage = (stats.e_attack * (bulletDamagePercentage / 100));
    }
    void Update()
    {
        if(canShoot)
        {
            transform.position += lastPlayerPosition * bulletSpeed * Time.deltaTime;
        }
        else if(!canShoot)
        {
            Vector3 direction = FindFirstObjectByType<Player>().transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(bulletDamage);

            GameObject explosionObj = Instantiate(explosionVFX, gameObject.transform.position, Quaternion.identity);
            Destroy(explosionObj, 2);
        }

        Destroy(gameObject);
    }
}
