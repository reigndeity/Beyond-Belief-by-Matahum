using UnityEngine;

public class MeteorShower_MeteorBullet : MonoBehaviour
{
    [HideInInspector]public Vector3 meteorPosition;
    public float meteorDamagePercentage;
    public float meteorDamage;
    public float meteorSpeed = 30;
    [HideInInspector]public bool startFalling;
    private Rigidbody rb;
    private EnemyStats stats;

    public FractureObject fractObj;
    public GameObject smokeVFX;

    [Header("Audio")]
    public CharacterAudioClip[] clips;

    private void Start()
    {
        stats = FindFirstObjectByType<Nuno>().GetComponent<EnemyStats>();
        meteorDamage = (stats.e_attack * (meteorDamagePercentage / 100));
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
