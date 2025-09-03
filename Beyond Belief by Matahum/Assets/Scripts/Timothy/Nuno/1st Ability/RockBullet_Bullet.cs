using UnityEngine;
using System.Collections;

public class RockBullet_Bullet : MonoBehaviour
{
    public Vector3 lastPlayerPosition;
    public float bulletDamagePercentage = 50;
    public float bulletDamage;
    public float bulletSpeed;
    public bool canShoot;

    private Nuno_Stats stats;

    // Update is called once per frame

    private void Start()
    {
        stats = FindFirstObjectByType<Nuno_Stats>();
        bulletDamage = (stats.n_attack * (bulletDamagePercentage / 100));
    }
    void Update()
    {
        if(canShoot)
        {
            transform.position += lastPlayerPosition * bulletSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(bulletDamage);
        }
        if(canShoot) Destroy(gameObject);
    }
}
