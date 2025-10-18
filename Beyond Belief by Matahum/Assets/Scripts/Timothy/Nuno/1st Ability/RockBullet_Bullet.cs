using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class RockBullet_Bullet : MonoBehaviour
{
    public Vector3 lastPlayerPosition;
    public float bulletDamagePercentage = 50;
    public float bulletDamage;
    public float bulletSpeed;
    public bool canShoot;

    private EnemyStats stats;
    public GameObject explosionVFX;
    public GameObject bulletAssetObject;

    [Header("Spin Settings")]
    public float startSpinSpeed = 0f;   // slow start
    public float endSpinSpeed = 180f;    // max spin speed
    public float spinDuration = 3f;      // time to ramp up spin

    private float spinTime;
    private float currentSpinSpeed;

    [Header("Audio")]
    public CharacterAudioClip[] clips;

    private void Start()
    {
        stats = FindFirstObjectByType<Nuno>().GetComponent<EnemyStats>();
        bulletDamage = (stats.e_attack * (bulletDamagePercentage / 100));

        spinTime = 0f; // reset
    }

    void Update()
    {
        if (canShoot)
        {
            transform.position += lastPlayerPosition * bulletSpeed * Time.deltaTime;
        }
        else
        {
            Vector3 direction = FindFirstObjectByType<Player>().transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }

        // Increase spinTime until spinDuration is reached
        spinTime += Time.deltaTime;
        float t = Mathf.Clamp01(spinTime / spinDuration);

        // Lerp spin speed
        currentSpinSpeed = Mathf.Lerp(startSpinSpeed, endSpinSpeed, t);

        // Spin around Z axis instead of Y
        bulletAssetObject.transform.Rotate(Vector3.forward, currentSpinSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(bulletDamage);

            GameObject explosionObj = Instantiate(explosionVFX, gameObject.transform.position, Quaternion.identity);
            AudioSource audio = explosionObj.GetComponent<AudioSource>();
            PlayAudio(audio, explosionObj);
            Destroy(explosionObj, 2);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
            other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Destroy(gameObject);
        }   
    }

    private void PlayAudio(AudioSource audioSource, GameObject obj)
    {
        AudioWrapper wrapper = obj.GetComponent<AudioWrapper>();

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
