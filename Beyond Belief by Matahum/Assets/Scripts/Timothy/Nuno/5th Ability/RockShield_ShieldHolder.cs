using System.Collections;
using UnityEngine;

public class RockShield_ShieldHolder : MonoBehaviour
{
    public RockShield_Shield[] shieldPrefab; // 4 shields in inspector
    public GameObject invulnerableShield;
    [HideInInspector] public Nuno nuno;
    [HideInInspector] public EnemyStats stats;
    [HideInInspector] public RockShield_NunoAbility ability;

    [HideInInspector] public float shieldHealth;
    [HideInInspector] public float shieldToHealthRatio;
    [HideInInspector] public int shieldCooldown = 10;
    public int maxShields = 4;

    public float orbitRadius = 2f;
    public float rotationSpeed = 30f;

    void Update()
    {
        if (IsAllShieldDestroyed() == false)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    public void Initialize(Nuno nuno, EnemyStats stats, float shieldToHealthRatio, int cooldown, RockShield_NunoAbility ability)
    {
        this.nuno = nuno;
        this.stats = stats;
        this.shieldToHealthRatio = shieldToHealthRatio;
        this.shieldCooldown = cooldown;
        this.ability = ability;

        shieldHealth = stats.e_maxHealth * (shieldToHealthRatio / 100);
    }

    public void ResetShield()
    {
        nuno.isVulnerable = false;
        nuno.gameObject.layer = LayerMask.NameToLayer("Default");
        for (int i = 0; i < shieldPrefab.Length; i++)
        {
            var shield = shieldPrefab[i];

            if (!shield.gameObject.activeSelf) // only restore missing shields
            {
                float angle = (360f / maxShields) * i;
                Vector3 pos = Quaternion.Euler(0, angle, 0) * Vector3.forward * orbitRadius;

                //shield.transform.SetParent(transform, false);
                shield.transform.localPosition = pos;

                shield.Init(this, shieldHealth);
            }
        }

        invulnerableShield.SetActive(!IsAllShieldDestroyed()); // only invulnerable if shields exist
    }

    public void OnShieldDestroyed()
    {
        if (IsAllShieldDestroyed() == true)
        {
            invulnerableShield.SetActive(false);
            nuno.isVulnerable = true;
            //Nuno_AttackManager.Instance.isStunned = true;
            Nuno_AttackManager.Instance.ApplyStun();
            nuno.gameObject.layer = LayerMask.NameToLayer("Enemy");
            ability.GoOnCooldown();
        }
    }

    public bool IsAllShieldDestroyed()
    {
        int isDestroyedShield = 0;

        for (int i = 0; i < shieldPrefab.Length; i++)
        {
            var shield = shieldPrefab[i];

            if (!shield.gameObject.activeSelf) // only restore missing shields
            {
                isDestroyedShield++;
            }
        }
        return isDestroyedShield == maxShields;
    }
}
