using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public enum Mangkukulam_DebuffType
{
    DamageDebuff,
    ArmorDebuff,
    PoisonDebuff
}

public class PotionBottle : MonoBehaviour
{
    public float bulletDamage;
    public Mangkukulam_DebuffType debuffType;
    public float debuffValue;
    public float debuffDuration = 5f;


    private void OnTriggerEnter(Collider other)
    {
        PlayerStats stats = other.GetComponent<PlayerStats>();
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject.tag == "Player")
        {
            //CheckForDebuffType(stats);

            damageable.TakeDamage(bulletDamage);
        }
        Destroy(gameObject);
    }

    void CheckForDebuffType(PlayerStats stats)
    {
        if (debuffType == Mangkukulam_DebuffType.DamageDebuff)
        {
            CoroutineRunner.Instance.RunCoroutine(DamageReduction(stats));
        }
        else if (debuffType == Mangkukulam_DebuffType.ArmorDebuff)
        {
            CoroutineRunner.Instance.RunCoroutine(ArmorReduction(stats));
        }
    }
    IEnumerator DamageReduction(PlayerStats stats)
    {
        int origValue = stats.p_attack;
        stats.p_attack -= (int)(stats.p_attack * (debuffValue / 100));
        if (stats.p_attack <= 0) stats.p_attack = 0;
        yield return new WaitForSeconds(debuffDuration);
        stats.p_attack = origValue;
    }

    IEnumerator ArmorReduction(PlayerStats stats)
    {
        float origValue = stats.p_defense;
        stats.p_defense -= stats.p_defense * (debuffValue / 100);
        if (stats.p_defense <= 0) stats.p_defense = 0;
        yield return new WaitForSeconds(debuffDuration);
        stats.p_defense = origValue;
    }

    IEnumerator Poison(PlayerStats stats)
    {
        float origValue = stats.p_defense;
        stats.p_defense -= stats.p_defense * (debuffValue / 100);
        if (stats.p_defense <= 0) stats.p_defense = 0;
        yield return new WaitForSeconds(debuffDuration);
        stats.p_defense = origValue;
    }
}
