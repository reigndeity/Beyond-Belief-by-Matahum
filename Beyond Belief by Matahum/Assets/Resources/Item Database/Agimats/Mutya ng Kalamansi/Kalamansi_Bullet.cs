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


    private void OnTriggerEnter(Collider other)
    {
        EnemyStats stats = other.GetComponent<EnemyStats>();
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject.tag != "Player")
        {
            CheckForDebuffType(stats);

            if(other.gameObject.tag == "Aswang")
                damageable.TakeDamage(bulletDamage * 1.5f);
            else
                damageable.TakeDamage(bulletDamage);

        }
        Destroy(gameObject);
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
