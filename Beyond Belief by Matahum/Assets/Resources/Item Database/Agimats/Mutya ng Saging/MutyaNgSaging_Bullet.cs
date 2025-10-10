using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MutyaNgSaging_Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float bulletDamage;
    private Vector3 moveDirection;
    public GameObject explosion_VFX;
    private float currentSpinSpeed = 480;

    private void Start()
    {
        Destroy(gameObject, 10);
    }
    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;      
        transform.Rotate(Vector3.forward, Random.Range(-currentSpinSpeed, currentSpinSpeed) * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject.tag != "Player")
        {
            float finalDamage = bulletDamage;

            EnemyStats stats = other.GetComponent<EnemyStats>();
            if (stats != null)
            {
                finalDamage += stats.e_defense * 0.66f;
            }

            damageable.TakeDamage(finalDamage);

            GameObject vfx = Instantiate(explosion_VFX, transform.position, Quaternion.identity);
            Destroy(vfx, 1.5f);
            Destroy(gameObject);
        }
    }
}

