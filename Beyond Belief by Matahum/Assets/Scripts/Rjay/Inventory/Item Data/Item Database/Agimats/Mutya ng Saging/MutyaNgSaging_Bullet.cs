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
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
    }
}

