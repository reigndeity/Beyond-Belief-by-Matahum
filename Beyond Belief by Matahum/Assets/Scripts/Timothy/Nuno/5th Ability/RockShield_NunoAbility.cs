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

    public bool canBeUsed = true;
    public void OnEnable()
    {
        canBeUsed = true;
    }
    public override bool CanBeUsed() => canBeUsed;
    public override void Activate()
    {
        canBeUsed = false;
        CoroutineRunner.Instance.RunCoroutine(DelaySpawnShield());
    }

    public override void Deactivate() { Debug.Log("Do Nothing. This basically is the cause why everything will stop"); }

    IEnumerator DelaySpawnShield()
    {
        yield return new WaitForSeconds(4f);

        // Look for existing shieldHolder
        //activeShieldHolder = Object.FindFirstObjectByType<RockShield_ShieldHolder>();

        if (activeShieldHolder == null)
        {
            Debug.Log("Shield not existing, Instantiating a new one...");
            Vector3 offset = new Vector3(0, 0.5f, 0);
            activeShieldHolder = Instantiate(
                shieldHolderPrefab,
                Nuno_AttackManager.Instance.transform.position + offset,
                Quaternion.identity,
                Nuno_AttackManager.Instance.transform.parent
            );
        }
        else Debug.Log("Shield exists...");

        // Call initialize manually BEFORE ResetShield
        var nuno = Nuno_AttackManager.Instance.GetComponent<Nuno>();
        var stats = Nuno_AttackManager.Instance.GetComponent<EnemyStats>();

        activeShieldHolder.Initialize(nuno, stats, shieldToNunoHealthRatio, shieldCooldown, this);

        activeShieldHolder.ResetShield();

    }
    public void GoOnCooldown()
    {
        CoroutineRunner.Instance.RunCoroutine(ShieldOnCooldown());
    }
    IEnumerator ShieldOnCooldown()
    {
        yield return new WaitForSeconds(shieldCooldown);
        canBeUsed = true;
    }
}
