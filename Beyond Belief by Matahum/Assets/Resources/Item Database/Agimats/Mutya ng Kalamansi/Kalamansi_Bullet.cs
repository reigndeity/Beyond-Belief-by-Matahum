using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public enum Kalamansi_DebuffType
{
    DamageDebuff,
    ArmorDebuff
}

public class Kalamansi_Bullet : MonoBehaviour
{
    public float bulletDamage;
    public Kalamansi_DebuffType debuffType;
    public float debuffValue;
    public float debuffDuration = 5f;
    public GameObject atkDown_xpld_VFX;
    public GameObject defDown_xpld_VFX;
    private GameObject vfxToSpawn;

    #region Rotator
    public float rotationSpeed = 2f;
    public float changeDirectionInterval = 2f;

    private Vector3 targetRotation;
    private float timer;

    void Start()
    {
        PickNewRotation();
        Destroy(gameObject, 10);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeDirectionInterval)
        {
            PickNewRotation();
            timer = 0f;
        }

        // Smoothly rotate towards the random rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(targetRotation),
            rotationSpeed * Time.deltaTime
        );
    }

    void PickNewRotation()
    {
        float randomX = UnityEngine.Random.Range(-40f, 40f);
        float randomY = UnityEngine.Random.Range(-40f, 40f);
        targetRotation = new Vector3(randomX, randomY, 0);
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject.tag != "Player")
        {
            float finalDamage = bulletDamage;

            EnemyStats stats = other.GetComponent<EnemyStats>();
            if (stats != null)
            {
                finalDamage += stats.e_defense * 0.66f;
                CheckForDebuffType(stats);
            }

            if (other.gameObject.tag == "Aswang")
                damageable.TakeDamage(finalDamage * 1.5f);
            else
                damageable.TakeDamage(finalDamage);

            CheckBuffForVFX(other.transform);
            Destroy(gameObject);
        }
    }

    void CheckBuffForVFX(Transform target)
    {
        if (debuffType == Kalamansi_DebuffType.DamageDebuff)
        {
            vfxToSpawn = Instantiate(atkDown_xpld_VFX, transform.position, Quaternion.identity, target.transform);
        }
        else if (debuffType == Kalamansi_DebuffType.ArmorDebuff)
        {
            vfxToSpawn = Instantiate(defDown_xpld_VFX, transform.position, Quaternion.identity, target.transform);
        }
        Destroy(vfxToSpawn, 1);
    }
    void CheckForDebuffType(EnemyStats stats)
    {
        if (debuffType == Kalamansi_DebuffType.DamageDebuff)
        {
            CoroutineRunner.Instance.RunCoroutine(DamageReduction(stats));
        }
        else if (debuffType == Kalamansi_DebuffType.ArmorDebuff)
        {
            CoroutineRunner.Instance.RunCoroutine(ArmorReduction(stats));
        }
    }
    IEnumerator DamageReduction(EnemyStats stats)
    {
        float origValue = stats.e_attack;
        stats.e_attack -= stats.e_attack * (debuffValue / 100);
        if (stats.e_attack <= 0) stats.e_attack = 0;
        yield return new WaitForSeconds(debuffDuration);
        stats.e_attack = origValue;
    }

    IEnumerator ArmorReduction(EnemyStats stats)
    {
        float origValue = stats.e_defense;
        stats.e_defense -= stats.e_defense * (debuffValue / 100);
        if (stats.e_defense <= 0) stats.e_defense = 0;
        yield return new WaitForSeconds(debuffDuration);
        stats.e_defense = origValue;
    }
}
