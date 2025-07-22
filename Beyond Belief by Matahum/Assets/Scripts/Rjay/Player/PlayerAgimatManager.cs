using UnityEngine;

public class PlayerAgimatManager : MonoBehaviour
{
    private Player m_player;
    private PlayerInput m_input;

    private float agimat1CooldownRemaining;
    private float agimat2CooldownRemaining;

    void Start()
    {
        m_player = GetComponent<Player>();
        m_input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        HandleCooldowns();
        HandleInput();
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

    public float GetCooldownRemaining(int slot)
    {
        return slot == 1 ? agimat1CooldownRemaining : agimat2CooldownRemaining;
    }

    public float GetCooldownMax(int slot)
    {
        var ability = m_player.GetAgimatAbility(slot);
        var rarity = m_player.GetAgimatRarity(slot);
        return ability?.GetCooldown(rarity) ?? 0f;
    }
}
