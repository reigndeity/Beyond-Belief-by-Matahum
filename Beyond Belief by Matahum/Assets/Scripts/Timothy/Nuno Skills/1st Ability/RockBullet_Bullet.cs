using UnityEngine;
using System.Collections;

public class RockBullet_Bullet : MonoBehaviour
{
    public Vector3 lastPlayerPosition;
    public float bulletDamage;
    public float bulletSpeed;
    public bool canShoot;

    // Update is called once per frame
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
        Destroy(gameObject);
    }
}
