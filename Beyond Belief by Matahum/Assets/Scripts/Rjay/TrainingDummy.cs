    using System.Collections;
    using UnityEngine.UI;
    using UnityEngine;

    public class TrainingDummy : MonoBehaviour, IDamageable
    {
        private Player m_player;
        private PlayerStats m_playerStats;
        [SerializeField] private float maxHealth;
        private float currentHealth;
        private BlazeAI m_blazeAI;
        [SerializeField] GameObject dummyMesh;
        private Collider m_collider;

        [Header("Dummy UI")]
        [SerializeField] private GameObject healthBarRoot;
        [SerializeField] Image healthFillInstant;
        [SerializeField] Image healthFillDelayed;
        [SerializeField] float healthFillDelaySpeed = 2f;
        private CanvasGroup healthBarCanvasGroup;
        private Coroutine healthBarFadeRoutine;
        [Header("Dummy Particle System")]
        [SerializeField] Transform dummyTransformPS;
        [SerializeField] ParticleSystem dummyDeathPS;
        

        void Awake()
        {
            m_blazeAI = GetComponent<BlazeAI>();
            m_playerStats = FindFirstObjectByType<PlayerStats>();
            m_player = FindFirstObjectByType<Player>();
            healthBarCanvasGroup = healthBarRoot.GetComponent<CanvasGroup>();
            currentHealth = maxHealth;
            m_collider = GetComponent<Collider>();
        }

        void Start()
        {
            HealthBarFadeIn(0.25f);
        }
        void Update()
        {
            UIHealthUpdate();
        }

        public void TakeDamage(float damage, bool hitAnimOn = true)
        {
            GetHit();

            bool isCriticalHit = UnityEngine.Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
            if (isCriticalHit)
                damage *= (1f + (m_playerStats.p_criticalDamage / 100f));

            int finalDamage = Mathf.Max(Mathf.FloorToInt(damage), 1);
            currentHealth -= finalDamage;

            Vector3 PopUpRandomness = new Vector3(Random.Range(0f, 0.25f),Random.Range(0f, 0.25f),Random.Range(0f, 0.25f));
            if (isCriticalHit)
            {
                DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.red);
            }
            else
            {
                DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.white);
            }

            if (currentHealth <= 0 && !isDead)
            {
                StartCoroutine(Dying());
            }  
        }

        public void GetHit()
        {
            m_blazeAI.Hit();
            FindFirstObjectByType<PlayerCamera>().CameraShake(0.1f, 1f);
            HitStop.Instance.TriggerHitStop(0.05f);
        }
        public bool isDead = false;
        public bool IsDead() => isDead;

        IEnumerator Dying()
        {
            HealthBarFadeOut(0.25f);
            isDead = true;
            currentHealth = 0;
            m_collider.enabled = false;
            m_collider.excludeLayers = LayerMask.NameToLayer("Player");
            var dsps = Instantiate(dummyDeathPS, dummyTransformPS);
            dsps.transform.SetParent(null);
            dummyMesh.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            m_blazeAI.Death();
        }
        void UIHealthUpdate()
        {
            float current = currentHealth;
            float max = maxHealth;

            float targetFill = current / max;

            healthFillInstant.fillAmount = targetFill;

            if (healthFillDelayed.fillAmount > targetFill)
            {
                healthFillDelayed.fillAmount = Mathf.Lerp(healthFillDelayed.fillAmount, targetFill, Time.deltaTime * healthFillDelaySpeed);
            }
            else
            {
                healthFillDelayed.fillAmount = targetFill;
            }
        }
        public void HealthBarFadeIn(float duration)
        {
            if (healthBarFadeRoutine != null) StopCoroutine(healthBarFadeRoutine);
            healthBarRoot.SetActive(true);
            healthBarFadeRoutine = StartCoroutine(FadeHealthBar(0f, 1f, duration));
        }

        public void HealthBarFadeOut(float duration)
        {
            if (healthBarFadeRoutine != null) StopCoroutine(healthBarFadeRoutine);
            healthBarFadeRoutine = StartCoroutine(FadeHealthBar(1f, 0f, duration));
        }
        private IEnumerator FadeHealthBar(float from, float to, float duration)
        {
            float currentAlpha = healthBarCanvasGroup.alpha;

            // Skip fade if already at or near the target
            if (Mathf.Approximately(currentAlpha, to))
            {
                healthBarCanvasGroup.alpha = to;
                healthBarRoot.SetActive(to > 0f);
                yield break;
            }

            float time = 0f;
            healthBarCanvasGroup.alpha = from;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                healthBarCanvasGroup.alpha = Mathf.Lerp(from, to, t);
                yield return null;
            }

            healthBarCanvasGroup.alpha = to;
            healthBarRoot.SetActive(to > 0f);

            healthBarFadeRoutine = null;
        }
    }
