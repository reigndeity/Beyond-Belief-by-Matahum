using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Nuno/Nuno Abilities/Rock Shield")]
public class RockShield_NunoAbility : Nuno_Ability
{
    public RockShield_ShieldHolder shieldHolderPrefab;
    private RockShield_ShieldHolder activeShieldHolder;

    [Tooltip("Percentage of Nuno's health is the health of each shield")]
    public float shieldToNunoHealthRatio;
    public int shieldCooldown = 10;

    public override void Activate()
    {
        CoroutineRunner.Instance.RunCoroutine(DelaySpawnShield());
    }

    IEnumerator DelaySpawnShield()
    {
        yield return new WaitForSeconds(1f);

        // Look for existing shieldHolder
        activeShieldHolder = Object.FindFirstObjectByType<RockShield_ShieldHolder>();

        if (activeShieldHolder == null)
        {
            // Instantiate at Nuno’s position
            activeShieldHolder = Instantiate(shieldHolderPrefab, Nuno_AttackManager.Instance.transform.position, Quaternion.identity, Nuno_AttackManager.Instance.transform.parent);
        }

        // Fill missing shields only
        activeShieldHolder.shieldCooldown = shieldCooldown;
        activeShieldHolder.shieldToHealthRatio = shieldToNunoHealthRatio;
        activeShieldHolder.ResetShield();
    }
}
