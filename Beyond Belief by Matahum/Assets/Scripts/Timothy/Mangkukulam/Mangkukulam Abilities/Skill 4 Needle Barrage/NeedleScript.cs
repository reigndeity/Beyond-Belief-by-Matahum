using UnityEngine;

public class NeedleScript : MonoBehaviour
{
    [HideInInspector] public Vector3 lastPlayerPosition;
    public float damagePercentage = 50;
    [HideInInspector] private float damage;
    [HideInInspector] public float speed;
    [HideInInspector] public bool canShoot;
    public GameObject particleFX;

    private EnemyStats stats;

    [Header("Audio")]
    public AudioSource audioSource;
    private bool isPlaying = false;
    public AudioClip[] clips;
    private bool isHit = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        stats = FindFirstObjectByType<Mangkukulam>().GetComponent<EnemyStats>();
        damage = (stats.e_attack * (damagePercentage / 100));
        Destroy(gameObject, 10f);
    }
    void Update()
    {
        if (canShoot)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                audioSource.clip = clips[0];
                audioSource.Play();
            }
            transform.position += lastPlayerPosition * speed * Time.deltaTime;
            particleFX.SetActive(true);
        }
        else if (!canShoot)
        {
            Vector3 direction = FindFirstObjectByType<Player>().transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction.normalized);
            particleFX.SetActive(false);
        }

        if (Time.timeScale == 0)
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
        else
        {
            if (!audioSource.isPlaying && audioSource.clip != null)
                audioSource.UnPause();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
            other.gameObject.layer == LayerMask.NameToLayer("Terrain") ||
            other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (isHit) return;

            isHit = true;
            int randomizer = Random.Range(1,clips.Length);
            audioSource.clip = clips[randomizer];
            audioSource.Play();
            float clipLength = clips[randomizer].length;
            Destroy(gameObject, clipLength);
        }
    }
}
