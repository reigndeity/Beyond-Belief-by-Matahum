using UnityEngine;

public class RockShield_Shield : MonoBehaviour, IDamageable
{
    [HideInInspector] public RockShield_ShieldHolder shieldHolder;
    public EnemyStats stats;
    private FractureObject vfx;

    private PlayerStats m_playerStats;

    [Header("Audio")]
    public CharacterAudioClip[] clips;

    void Start()
    {
        m_playerStats = FindFirstObjectByType<PlayerStats>();  
        vfx = GetComponentInParent<FractureObject>();
    }
    public bool IsDead() => stats.e_currentHealth <= 0;

    public void Init(RockShield_ShieldHolder holder, float health)
    {
        gameObject.SetActive(true);

        stats = GetComponent<EnemyStats>();

        shieldHolder = holder;
        stats.e_maxHealth = health;
        stats.RecalculateStats();
        
    }

    public void TakeDamage(float damage, bool hitAnimOn = true)
    {
        stats.e_currentHealth -= damage;

        PopUpDamage(damage);

        if (IsDead())
        {
            vfx.Explode();
            PlayAudio();
            stats.e_currentHealth = 0f;
            shieldHolder.OnShieldDestroyed();
        }
    }

    private void PopUpDamage(float damage)
    {
        bool isCriticalHit = UnityEngine.Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
        int finalDamage = Mathf.Max(Mathf.FloorToInt(damage), 1);
        Vector3 PopUpRandomness = new Vector3(Random.Range(0f, 0.25f), Random.Range(0f, 0.25f), Random.Range(0f, 0.25f));
        if (isCriticalHit)
        {
            DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.red);
        }
        else
        {
            DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.white);
        }
    }

    private void PlayAudio()
    {
        AudioSource audioSource = vfx.explosionSFX;
        AudioWrapper wrapper = vfx.GetComponent<AudioWrapper>();

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
