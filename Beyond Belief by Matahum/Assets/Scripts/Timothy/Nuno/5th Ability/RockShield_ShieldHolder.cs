using System.Collections;
using UnityEngine;

public class RockShield_ShieldHolder : MonoBehaviour
{
    public RockShield_Shield[] shieldPrefab; // 4 shields in inspector
    public GameObject invulnerableShield;
    private Nuno nuno;
    private Nuno_Stats stats;

    [HideInInspector] public float shieldHealth;
    [HideInInspector] public float shieldToHealthRatio;
    [HideInInspector] public int shieldCooldown = 10;
    private bool canUseShield = true;
    private int maxShields = 4;

    public float orbitRadius = 2f;
    public float rotationSpeed = 30f;

    private void Start()
    {
        stats = FindFirstObjectByType<Nuno_Stats>();
        nuno = FindFirstObjectByType<Nuno>();
        shieldHealth = stats.n_maxHealth * (shieldToHealthRatio / 100);
    }
    void Update()
    {
        if (IsAllShieldDestroyed() == false)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    public void ResetShield()
    {
        if (nuno.isVulnerable == false || !canUseShield) return;

        nuno.isVulnerable = false;
        nuno.gameObject.layer = LayerMask.NameToLayer("Default");
        for (int i = 0; i < shieldPrefab.Length; i++)
        {
            var shield = shieldPrefab[i];

            if (!shield.gameObject.activeSelf) // only restore missing shields
            {
                float angle = (360f / maxShields) * i;
                Vector3 pos = Quaternion.Euler(0, angle, 0) * Vector3.forward * orbitRadius;

                shield.transform.SetParent(transform, false);
                shield.transform.localPosition = pos;

                shield.Init(this, shieldHealth);
            }
        }

        invulnerableShield.SetActive(!IsAllShieldDestroyed()); // only invulnerable if shields exist
    }

    IEnumerator ShieldOnCooldown()
    {
        canUseShield = false;
        yield return new WaitForSeconds(shieldCooldown);
        canUseShield = true;
    }
    public void OnShieldDestroyed()
    {
        if (IsAllShieldDestroyed() == true)
        {
            invulnerableShield.SetActive(false);
            nuno.isVulnerable = true;
            nuno.gameObject.layer = LayerMask.NameToLayer("Enemy");
            StartCoroutine(ShieldOnCooldown());
            Debug.Log("All shields destroyed → Boss vulnerable!");
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
