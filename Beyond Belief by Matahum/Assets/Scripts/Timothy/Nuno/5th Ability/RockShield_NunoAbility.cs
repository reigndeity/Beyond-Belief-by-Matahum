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
        yield return new WaitForSeconds(4f);

        // Look for existing shieldHolder
        activeShieldHolder = Object.FindFirstObjectByType<RockShield_ShieldHolder>();

        if (activeShieldHolder == null)
        {
            Vector3 offset = new Vector3(0, 0.5f, 0);
            activeShieldHolder = Instantiate(
                shieldHolderPrefab,
                Nuno_AttackManager.Instance.transform.position + offset,
                Quaternion.identity,
                Nuno_AttackManager.Instance.transform.parent
            );
        }

        // Call initialize manually BEFORE ResetShield
        var nuno = Object.FindFirstObjectByType<Nuno>();
        var stats = Object.FindFirstObjectByType<Nuno_Stats>();
        activeShieldHolder.Initialize(nuno, stats, shieldToNunoHealthRatio, shieldCooldown);

        activeShieldHolder.ResetShield();

    }
}
