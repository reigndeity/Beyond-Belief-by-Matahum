using UnityEngine;

public class MutyaNgSaging_Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float bulletDamage;
    private Vector3 moveDirection;

    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject.tag != "Player")
        {
            damageable.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
    }
}

