using UnityEngine;
using System.Collections;

public class AudioZone : MonoBehaviour
{
    // ===============================
    // 🎵 MUSIC ZONE
    // ===============================
    [Header("🎵 Music Zone")]
    public bool enableMusicZone = true;
    public AudioSource musicSource;          // assign manually
    public AudioClip musicClip;
    [Range(0f, 1f)] public float musicTargetVolume = 1f;
    public float musicFadeDuration = 2f;
    public float musicZoneRadius = 15f;
    public bool resetMusicOnExit = false;
    private Coroutine musicFadeCoroutine;
    private bool playerInMusic = false;

    // ===============================
    // 🌿 AMBIENCE ZONE
    // ===============================
    [Header("🌿 Ambience Zone")]
    public bool enableAmbienceZone = true;
    public AudioSource ambienceSource;       // assign manually
    public AudioClip ambienceClip;
    [Range(0f, 1f)] public float ambienceTargetVolume = 1f;
    public float ambienceFadeDuration = 2f;
    public float ambienceZoneRadius = 25f;
    public bool resetAmbienceOnExit = true;
    private Coroutine ambienceFadeCoroutine;
    private bool playerInAmbience = false;

    // ===============================
    // 🧠 SETTINGS
    // ===============================
    [Header("🧠 Settings")]
    public string playerTag = "Player";
    public int zonePriority = 0;

    private Transform player;
    private static AudioZone currentActiveZone;

    private void OnEnable()
    {
        AudioManager.OnDangerStateChanged += HandleDangerState; // listen for danger events
    }

    private void OnDisable()
    {
        AudioManager.OnDangerStateChanged -= HandleDangerState;

        // cleanup
        if (currentActiveZone == this)
            currentActiveZone = null;
    }

    private void Awake()
    {
        if (enableMusicZone && musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
        }

        if (enableAmbienceZone && ambienceSource != null && ambienceClip != null)
        {
            ambienceSource.clip = ambienceClip;
            ambienceSource.loop = true;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
            else return;
        }

        float dist = Vector3.Distance(player.position, transform.position);
        bool insideMusic = enableMusicZone && musicSource != null && dist <= musicZoneRadius;
        bool insideAmbience = enableAmbienceZone && ambienceSource != null && dist <= ambienceZoneRadius;
        bool insideAny = insideMusic || insideAmbience;

        // handle activation logic
        if (insideAny)
        {
            if (currentActiveZone == null)
            {
                ActivateZone();
            }
            else if (currentActiveZone != this)
            {
                float myDist = dist;
                float theirDist = Vector3.Distance(player.position, currentActiveZone.transform.position);

                bool iShouldTakeOver = zonePriority > currentActiveZone.zonePriority ||
                                       (zonePriority == currentActiveZone.zonePriority && myDist < theirDist);

                if (iShouldTakeOver)
                {
                    currentActiveZone.DeactivateZone();
                    ActivateZone();
                }
            }
        }
        else
        {
            if (currentActiveZone == this)
                DeactivateZone();
        }
    }

    private void ActivateZone()
    {
        currentActiveZone = this;

        if (enableMusicZone && musicSource != null)
            FadeIn(musicSource, musicTargetVolume, musicFadeDuration);

        if (enableAmbienceZone && ambienceSource != null)
            FadeIn(ambienceSource, ambienceTargetVolume, ambienceFadeDuration);
    }

    private void DeactivateZone()
    {
        if (enableMusicZone && musicSource != null)
            FadeOut(musicSource, musicFadeDuration, resetMusicOnExit);

        if (enableAmbienceZone && ambienceSource != null)
            FadeOut(ambienceSource, ambienceFadeDuration, resetAmbienceOnExit);

        if (currentActiveZone == this)
            currentActiveZone = null;
    }

    private void FadeIn(AudioSource source, float targetVolume, float duration)
    {
        if (source.clip == null) return;

        if (!source.isPlaying)
            source.Play();

        StopFade(source);
        StartFade(source, targetVolume, duration, false);
    }

    private void FadeOut(AudioSource source, float duration, bool resetAfter)
    {
        if (source.clip == null) return;

        StopFade(source);
        StartFade(source, 0f, duration, resetAfter);
    }

    private void StopFade(AudioSource source)
    {
        if (source == musicSource && musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);
        if (source == ambienceSource && ambienceFadeCoroutine != null)
            StopCoroutine(ambienceFadeCoroutine);
    }

    private void StartFade(AudioSource source, float target, float duration, bool resetAfter)
    {
        Coroutine routine = StartCoroutine(FadeAudio(source, target, duration, resetAfter));
        if (source == musicSource) musicFadeCoroutine = routine;
        else ambienceFadeCoroutine = routine;
    }

    private IEnumerator FadeAudio(AudioSource source, float target, float duration, bool resetAfter)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, target, elapsed / duration);
            yield return null;
        }

        source.volume = target;

        if (Mathf.Approximately(target, 0f))
        {
            if (resetAfter)
            {
                source.Stop(); // resets playback to 0
            }
            else
            {
                // keep playing silently (preserves playback position)
                source.volume = 0f;
            }
        }
    }

    // ========================================
    // 🔥 HANDLE PLAYER DANGER MUSIC EVENTS
    // ========================================
    private void HandleDangerState(bool inDanger)
    {
        // only react if this zone is the active one
        if (currentActiveZone != this) return;

        if (musicSource == null) return;

        StopAllCoroutines();
        StartCoroutine(FadeZoneMusic(inDanger));
    }

    private IEnumerator FadeZoneMusic(bool inDanger)
    {
        float duration = 1.5f;
        float startVolume = musicSource.volume;
        float targetVolume = inDanger ? 0f : musicTargetVolume;
        float elapsed = 0f;

        if (!inDanger && !musicSource.isPlaying && musicClip != null)
        {
            // Restart music when returning from danger
            musicSource.clip = musicClip;
            musicSource.Play();
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;

        if (inDanger)
        {
            // stop it completely after fade out
            musicSource.Stop();
        }
    }

    // ========================================
    // 🎨 DEBUG VISUALS
    // ========================================
    private void OnDrawGizmosSelected()
    {
        if (enableMusicZone)
        {
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.35f); // Blue fill for music
            Gizmos.DrawSphere(transform.position, musicZoneRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, musicZoneRadius);
        }

        if (enableAmbienceZone)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.35f); // Red fill for ambience
            Gizmos.DrawSphere(transform.position, ambienceZoneRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ambienceZoneRadius);
        }
    }
}
