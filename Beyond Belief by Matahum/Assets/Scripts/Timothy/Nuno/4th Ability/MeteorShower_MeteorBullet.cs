using UnityEngine;

public class MeteorShower_MeteorBullet : MonoBehaviour
{
    public Vector3 meteorPosition;
    public float meteorDamage;
    public float meteorSpeed = 30;
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
        if(startFalling)
            rb.AddForce(Physics.gravity * (meteorSpeed - 1f), ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(meteorDamage);
        }
    }
}
