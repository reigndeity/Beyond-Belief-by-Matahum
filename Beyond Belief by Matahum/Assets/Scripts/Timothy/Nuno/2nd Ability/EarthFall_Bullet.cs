using UnityEngine;

public class EarthFall_Bullet : MonoBehaviour
{
    public float bulletDamage;
    public float bulletSpeed = 30;
    public bool startFalling;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Apply additional gravity manually
        if (startFalling)
            rb.AddForce(Physics.gravity * (bulletSpeed - 1f), ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(bulletDamage);
        }
    }
}
