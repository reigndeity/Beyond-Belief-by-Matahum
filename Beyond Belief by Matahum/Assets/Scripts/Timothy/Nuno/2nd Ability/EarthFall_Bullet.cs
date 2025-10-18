using UnityEngine;

public class EarthFall_Bullet : MonoBehaviour
{
    public float bulletDamagePercentage;
    public float bulletDamage;
    public float bulletSpeed = 30;
    public bool startFalling;
    private Rigidbody rb;
    private EnemyStats stats;

    public FractureObject fractObj;
    public GameObject smokeVFX;

    [Header("Audio")]
    public CharacterAudioClip[] clips;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        stats = FindFirstObjectByType<Nuno>().GetComponent<EnemyStats>();
        bulletDamage = (stats.e_attack * (bulletDamagePercentage / 100));
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

        if (other.gameObject.CompareTag("Nuno_Indicator"))
        {
            fractObj.Explode();
            PlayAudio();

            GameObject smokeVFXObj = Instantiate(smokeVFX, gameObject.transform.position, Quaternion.identity);
            Destroy(smokeVFXObj, 2);
        }
    }

    private void PlayAudio()
    {
        AudioSource audioSource = fractObj.explosionSFX;
        AudioWrapper wrapper = fractObj.GetComponent<AudioWrapper>();

        int randomizer = Random.Range(0, clips.Length);
        audioSource.clip = clips[randomizer].clip;
        audioSource.volume = AdjustVolume(clips[randomizer], wrapper);
        audioSource.Play();
    }

    private float AdjustVolume(CharacterAudioClip clip, AudioWrapper wrapper)
    {
        if (clip == null)
            return 1;

        // Get base clip volume (0–1 preferred)
        float clipVolume = Mathf.Clamp01(clip.volume);

        // Scale it by wrapper's max volume range
        float finalVolume = clipVolume * (wrapper != null ? wrapper.maxVolume : 1f);

        return finalVolume;
    }
}
