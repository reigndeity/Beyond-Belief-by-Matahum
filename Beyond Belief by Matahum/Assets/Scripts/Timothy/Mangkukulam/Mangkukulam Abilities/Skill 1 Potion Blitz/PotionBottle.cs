using UnityEngine;

public class PotionBottle : MonoBehaviour
{
    public float bulletDamage;
    public GameObject poisonArea;
    public GameObject explosionVFX;

    #region Rotator
    public float rotationSpeed = 2f;
    public float changeDirectionInterval = 2f;

    private Vector3 targetRotation;
    private float timer;

    void Start()
    {
        PickNewRotation();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeDirectionInterval)
        {
            PickNewRotation();
            timer = 0f;
        }

        // Smoothly rotate towards the random rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(targetRotation),
            rotationSpeed * Time.deltaTime
        );
    }

    void PickNewRotation()
    {
        float randomX = Random.Range(-130f, -50f);
        float randomZ = Random.Range(-40f, 40f);
        targetRotation = new Vector3(randomX, 0f, randomZ);
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.CompareTag("Player"))
        {
            damageable.TakeDamage(bulletDamage);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
            other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Vector3 offset = new Vector3(0, 0.1f, 0);
            Instantiate(poisonArea, transform.position + offset, Quaternion.identity);
            GameObject glassExplode = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(glassExplode,2);
            Destroy(gameObject);
        }
    }
}
