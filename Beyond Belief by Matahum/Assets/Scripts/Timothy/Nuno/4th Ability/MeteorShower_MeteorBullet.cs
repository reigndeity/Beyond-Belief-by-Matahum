using UnityEngine;

public class MeteorShower_MeteorBullet : MonoBehaviour
{
    [HideInInspector]public Vector3 meteorPosition;
    public float meteorDamagePercentage;
    public float meteorDamage;
    public float meteorSpeed = 30;
    [HideInInspector]public bool startFalling;
    private Rigidbody rb;
    private Nuno_Stats stats;

    private void Start()
    {
        stats = FindFirstObjectByType<Nuno_Stats>();
        meteorDamage = (stats.n_attack * (meteorDamagePercentage / 100));
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
