using UnityEngine;

public class PlayerAgimatManager : MonoBehaviour
{
    private Player m_player;
    private PlayerInput m_input;

    private float agimat1CooldownRemaining;
    private float agimat2CooldownRemaining;

    private float passive1Timer;
    private float passive1Buffer;
    private float passive2Timer;
    private float passive2Buffer;
    

    void Start()
    {
        m_player = GetComponent<Player>();
        m_input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        HandleCooldowns();
        HandleInput();
        HandlePassiveAgimat(1);
        HandlePassiveAgimat(2);
    }

    private void HandleCooldowns()
    {
        if (agimat1CooldownRemaining > 0f)
            agimat1CooldownRemaining -= Time.deltaTime;

        if (agimat2CooldownRemaining > 0f)
            agimat2CooldownRemaining -= Time.deltaTime;
    }

    private void HandleInput()
    {
        var agimat1 = m_player.GetAgimatAbility(1);
        var rarity1 = m_player.GetAgimatRarity(1);

        if (agimat1 != null && !agimat1.isPassive && agimat1CooldownRemaining <= 0f &&
            Input.GetKeyDown(m_input.agimatOneKey))
        {
            agimat1.Activate(gameObject, rarity1);
            agimat1CooldownRemaining = agimat1.GetCooldown(rarity1);
        }

        var agimat2 = m_player.GetAgimatAbility(2);
        var rarity2 = m_player.GetAgimatRarity(2);

        if (agimat2 != null && !agimat2.isPassive && agimat2CooldownRemaining <= 0f &&
            Input.GetKeyDown(m_input.agimatTwoKey))
        {
            agimat2.Activate(gameObject, rarity2);
            agimat2CooldownRemaining = agimat2.GetCooldown(rarity2);
        }
    }

    private void HandlePassiveAgimat(int slot)
    {
        var agimat = m_player.GetAgimatAbility(slot);
        var rarity = m_player.GetAgimatRarity(slot);

        if (agimat == null || !agimat.isPassive)
            return;

        float interval = agimat.GetCooldown(rarity);
        float delay = agimat.passiveTriggerDelay;

        if (slot == 1)
        {
            passive1Timer += Time.deltaTime;

            if (passive1Timer >= interval)
            {
                passive1Buffer += Time.deltaTime;

                if (passive1Buffer >= delay)
                {
                    agimat.Activate(gameObject, rarity);
                    passive1Timer = 0f;
                    passive1Buffer = 0f;
                }
            }
        }
        else
        {
            passive2Timer += Time.deltaTime;

            if (passive2Timer >= interval)
            {
                passive2Buffer += Time.deltaTime;

                if (passive2Buffer >= delay)
                {
                    agimat.Activate(gameObject, rarity);
                    passive2Timer = 0f;
                    passive2Buffer = 0f;
                }
            }
        }
    }


    public float GetCooldownRemaining(int slot)
    {
        var agimat = m_player.GetAgimatAbility(slot);
        if (agimat == null) return 0f;

        if (agimat.isPassive)
        {
            return slot == 1 ? agimat.GetCooldown(m_player.GetAgimatRarity(1)) - passive1Timer
                             : agimat.GetCooldown(m_player.GetAgimatRarity(2)) - passive2Timer;
        }
        else
        {
            return slot == 1 ? agimat1CooldownRemaining : agimat2CooldownRemaining;
        }
    }

    public float GetCooldownMax(int slot)
    {
        var ability = m_player.GetAgimatAbility(slot);
        var rarity = m_player.GetAgimatRarity(slot);
        return ability?.GetCooldown(rarity) ?? 0f;
    }
}
