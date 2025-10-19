using UnityEngine;
using System.Collections;

public class AudioZone : MonoBehaviour
{
    [Header("🎵 Music Zone")]
    public bool enableMusicZone = true;
    public AudioSource musicSource;
    public AudioClip musicClip;
    [Range(0f, 1f)] public float musicTargetVolume = 1f;
    public float musicFadeDuration = 2f;
    public float musicZoneRadius = 15f;
    public bool resetMusicOnExit = false;
    private Coroutine musicFadeCoroutine;
    private bool playerInMusic = false;

    [Header("🌿 Ambience Zone")]
    public bool enableAmbienceZone = true;
    public AudioSource ambienceSource;
    public AudioClip ambienceClip;
    [Range(0f, 1f)] public float ambienceTargetVolume = 1f;
    public float ambienceFadeDuration = 2f;
    public float ambienceZoneRadius = 25f;
    public bool resetAmbienceOnExit = true;
    private Coroutine ambienceFadeCoroutine;
    private bool playerInAmbience = false;

    [Header("🧠 Settings")]
    public string playerTag = "Player";
    public int zonePriority = 0;

    private Transform player;
    private static AudioZone currentActiveZone;

    // ⚙️ Wrappers to sync volume correctly
    private AudioWrapper musicWrapper;
    private AudioWrapper ambienceWrapper;

    private void OnEnable()
    {
        AudioManager.OnDangerStateChanged += HandleDangerState;

        // ⚙️ Subscribe to volume change events
        if (AudioManager.instance != null)
        {
            AudioManager.instance.OnBGMVolumeChanged += UpdateMusicVolumeFromManager;
            AudioManager.instance.OnBGMVolumeChanged += UpdateAmbienceVolumeFromManager;
        }
    }

    private void OnDisable()
    {
        AudioManager.OnDangerStateChanged -= HandleDangerState;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.OnBGMVolumeChanged -= UpdateMusicVolumeFromManager;
            AudioManager.instance.OnBGMVolumeChanged -= UpdateAmbienceVolumeFromManager;
        }

        if (currentActiveZone == this)
            currentActiveZone = null;
    }

    private void Awake()
    {
        // ⚙️ Auto-grab wrappers
        if (musicSource != null)
            musicWrapper = musicSource.GetComponent<AudioWrapper>();
        if (ambienceSource != null)
            ambienceWrapper = ambienceSource.GetComponent<AudioWrapper>();

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

    private void Start()
    {
        // ⚙️ Apply saved PlayerPrefs volume at startup
        if (AudioManager.instance != null)
        {
            float savedBGM = PlayerPrefs.GetFloat("Audio_BGM", 1f);
            float savedSFX = PlayerPrefs.GetFloat("Audio_SFX", 1f);

            UpdateMusicVolumeFromManager(savedBGM);
            UpdateAmbienceVolumeFromManager(savedBGM);
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
            FadeIn(musicSource, GetWrapperVolume(musicWrapper, musicTargetVolume), musicFadeDuration);

        if (enableAmbienceZone && ambienceSource != null)
            FadeIn(ambienceSource, GetWrapperVolume(ambienceWrapper, ambienceTargetVolume), ambienceFadeDuration);
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

    // =====================================
    // ⚙️ Helper to respect AudioWrapper volume scaling
    // =====================================
    private float GetWrapperVolume(AudioWrapper wrapper, float localTarget)
    {
        if (wrapper == null)
            return localTarget;

        // scaledVolume is already mapped via wrapper.ChangeVolume()
        return wrapper.GetCurrentVolume() * localTarget;
    }

    private void UpdateMusicVolumeFromManager(float newValue)
    {
        if (musicWrapper != null && musicSource != null)
            musicSource.volume = GetWrapperVolume(musicWrapper, musicTargetVolume);
    }

    private void UpdateAmbienceVolumeFromManager(float newValue)
    {
        if (ambienceWrapper != null && ambienceSource != null)
            ambienceSource.volume = GetWrapperVolume(ambienceWrapper, ambienceTargetVolume);
    }

    // =====================================

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
                source.Stop();
            else
                source.volume = 0f;
        }
    }

    private void HandleDangerState(bool inDanger)
    {
        if (currentActiveZone != this) return;
        if (musicSource == null) return;

        StopAllCoroutines();
        StartCoroutine(FadeZoneMusic(inDanger));
    }

    private IEnumerator FadeZoneMusic(bool inDanger)
    {
        float duration = 1.5f;
        float startVolume = musicSource.volume;
        float targetVolume = inDanger ? 0f : GetWrapperVolume(musicWrapper, musicTargetVolume);
        float elapsed = 0f;

        if (!inDanger && !musicSource.isPlaying && musicClip != null)
        {
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
            musicSource.Stop();
    }

    private void OnDrawGizmosSelected()
    {
        if (enableMusicZone)
        {
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.35f);
            Gizmos.DrawSphere(transform.position, musicZoneRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, musicZoneRadius);
        }

        if (enableAmbienceZone)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
            Gizmos.DrawSphere(transform.position, ambienceZoneRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ambienceZoneRadius);
        }
    }
}
