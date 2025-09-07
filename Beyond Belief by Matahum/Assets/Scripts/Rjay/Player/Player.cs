using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum PlayerState 
{
    IDLE,
    MOVING,
    JUMPING,
    FALLING,
    LANDING,
    COMBAT,
    DASHING
}
public class Player : MonoBehaviour, IDamageable
{
    private PlayerAnimator m_playerAnimator;
    private PlayerMovement m_playerMovement;
    private PlayerInput m_playerInput;
    private PlayerCombat m_playerCombat;
    private PlayerSkills m_playerSkills;
    private PlayerStats m_playerStats;
    
    private PlayerMinimap m_playerMinimap;
    private PlayerCamera m_playerCamera;
    private PlayerPamanaSetBonus m_setBonus;

    [Header("Player States")]
    public PlayerState currentState;
    public bool isDead = false;

    [Header("Inventory Properties")]
    public R_Inventory playerInventory;

    [HideInInspector] public bool suppressInputUntilNextFrame = false;

    
    void Awake()
    {
        foreach (R_PamanaSlotType slot in System.Enum.GetValues(typeof(R_PamanaSlotType)))
            equippedPamanaSet[slot] = null;
    }
    void Start()
    {
        m_playerAnimator = GetComponent<PlayerAnimator>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerInput = GetComponent<PlayerInput>();
        m_playerCombat = GetComponent<PlayerCombat>();
        m_playerSkills = GetComponent<PlayerSkills>();
        m_playerStats = GetComponent<PlayerStats>();
        m_playerMinimap = GetComponentInChildren<PlayerMinimap>();
        m_playerCamera = FindFirstObjectByType<PlayerCamera>();
        m_setBonus = GetComponent<PlayerPamanaSetBonus>();
    }

    void Update()
    {   
        if (UI_TransitionController.instance.isTeleporting) return;
        
        // Player Camera
        m_playerCamera.HandleMouseLock();
        if (Cursor.lockState == CursorLockMode.Locked)
            m_playerCamera.HandleRotation();
        if (!m_playerMinimap.IsMapOpen())
        {
            m_playerCamera.HandleZoom();
            m_playerMinimap.projectedViewIcon.enabled = true; // show when map is closed
        }
        else
        {
            m_playerMinimap.projectedViewIcon.enabled = false; // hide when map is open
        }

        // Map
        m_playerMinimap.ProjectionRotation();
        m_playerMinimap.HandleMapToggle();
        m_playerMinimap.ZoomControl();
        
        if (!isLocked && !m_playerMinimap.IsMapOpen() && !DialogueManager.Instance.isDialoguePlaying)
        {
            m_playerMovement.HandleMovement();
            m_playerMovement.HandleDash();
            m_playerMovement.HandleJump();
            m_playerCombat.HandleAttack();
            m_playerSkills.HandleSkills();
            HandleGrassInteraction();
            
        }


        float currentSpeed = m_playerMovement.Speed;
        m_playerAnimator.lastFrameSpeed = currentSpeed;
        m_playerAnimator.HandleAnimations();

        // --- Determine current player state ---
        if (m_playerMovement.IsDashing())
        {
            currentState = PlayerState.DASHING;
        }
        else if (m_playerCombat.IsAttacking())
        {
            currentState = PlayerState.COMBAT;
        }
        else if (m_playerMovement.JustLanded())
        {
            currentState = PlayerState.LANDING;
        }
        else if (!m_playerMovement.GetComponent<CharacterController>().isGrounded)
        {
            float verticalVel = m_playerMovement.GetVerticalVelocity();
            currentState = verticalVel < -1f ? PlayerState.FALLING : PlayerState.JUMPING;
        }
        else if (currentSpeed > 0.1f)
        {
            currentState = PlayerState.MOVING;
        }
        else
        {
            currentState = PlayerState.IDLE;
        }


        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(50);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            GainXP(5000);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            GainWeaponXP(500);
        }

        if (suppressInputUntilNextFrame)
            suppressInputUntilNextFrame = false;
    }

    #region PLAYER BEHAVIOR
    public void ForceIdleOverride()
    {
        suppressInputUntilNextFrame = true;

        m_playerMovement.ForceStop();
        m_playerCombat.ForceStopCombat();
        m_playerSkills.ForceStopSkills();
        m_playerAnimator.ForceIdleState();

        // If in mid-air, apply gravity until grounded
        if (!GetComponent<CharacterController>().isGrounded)
        {
            StartCoroutine(ApplyGravityUntilGrounded());
        }

        Debug.Log("🧍 ForceIdleOverride: Player forcibly reset to idle.");
    }


    private IEnumerator ApplyGravityUntilGrounded()
    {
        while (!GetComponent<CharacterController>().isGrounded)
        {
            m_playerMovement.ApplyGravityStep(); // Your built-in gravity application
            yield return null;
        }

        m_playerAnimator.ChangeAnimationState("player_idle_1"); // ensure idle once landed
    }
    private bool isLocked = false;
    public void SetPlayerLocked(bool locked)
    {
        isLocked = locked;

        if (isLocked)
        {
            // Immediately stop any current actions
            m_playerMovement.ForceStop();
            m_playerCombat.ForceStopCombat();
            m_playerSkills.ForceStopSkills();
            m_playerAnimator.ForceIdleState();
        }
        else // THIS ELSE BLOCK IS ALSO NEW IN AN ATTEMPT TO FIX THE SLIDING BUG 09/07/2025 - 2:23PM
        {
            m_playerAnimator.HandleAnimations();
        }

        Debug.Log($"🔒 Player locked: {isLocked}");
    }

    #endregion


    #region DAMAGE / HEAL FUNCTIONS
    public void TakeDamage(float damage)
    {
        if (m_playerSkills.isUsingUltimateSkill) return;
        
        bool isCriticalHit = Random.value <= (m_playerStats.p_criticalRate / 100f); // Crit Check
        float damageReduction = m_playerStats.p_defense * 0.66f; // Defense Scaling
        float reducedDamage = damage - damageReduction; // Damage after Defense
        if (isCriticalHit)
        {
            reducedDamage *= (1f + (m_playerStats.p_criticalDamage / 100f)); // Crit multiplier if critical hit
        }
        int finalDamage = Mathf.Max(Mathf.FloorToInt(reducedDamage), 1); // If defense is greater, cap the damage at 1

        m_playerStats.p_currentHealth -= finalDamage; // Final Damage
        m_playerStats.p_currentHealth = Mathf.Clamp(m_playerStats.p_currentHealth, 0f, m_playerStats.p_maxHealth); // Health cannot go below 0

        Vector3 PopUpRandomness = new Vector3(Random.Range(0f, 0.25f),Random.Range(0f, 0.25f),Random.Range(0f, 0.25f));
        if (isCriticalHit) // Damage Pop Up Here
        {
            DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.red);
            Debug.Log($"💥 CRITICAL HIT! Player took {finalDamage} damage. Current Health: {m_playerStats.p_currentHealth}");
        }
        else
        {
            
            DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, finalDamage.ToString(), Color.white);
            Debug.Log($"Player took {finalDamage} damage. Current Health: {m_playerStats.p_currentHealth}");
        }

        if (m_playerStats.p_currentHealth <= 0f && !isDead) // Death check
        {
            Debug.Log("Player is dead.");
            HandleDeath();
        }
        if (m_playerSkills.isUsingNormalSkill) return;
        m_playerAnimator.GetHit();
    }
    public void Heal(float amount)
    {
        if (amount <= 0f || isDead)
            return;

        float maxHP = m_playerStats.p_maxHealth;
        float oldHP = m_playerStats.p_currentHealth;

        m_playerStats.p_currentHealth = Mathf.Min(oldHP + amount, maxHP);

        int displayAmount = Mathf.Max(1, Mathf.FloorToInt(amount));

        Vector3 PopUpRandomness = new Vector3(Random.Range(0f, 0.25f),Random.Range(0f, 0.25f),Random.Range(0f, 0.25f));
        DamagePopUpGenerator.instance.CreatePopUp(transform.position + PopUpRandomness, displayAmount.ToString(), Color.green);

        Debug.Log($"💚 Healed {amount} HP. Current Health: {m_playerStats.p_currentHealth} / {maxHP}");
    }
    #region DEATH
    public async void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 Player has died.");

        // Play death animation if you have one
        SetPlayerLocked(true);
        m_playerAnimator.ChangeAnimationState("player_death");
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        await System.Threading.Tasks.Task.Delay(1000);
        Loader.Load(4);
        // Fade back in
    }
    #endregion

    #endregion

    #region PLAYER EXP
    public void GainXP(int amount)
    {
        m_playerStats.currentExp += amount;

        while (m_playerStats.currentLevel < 50 &&
            m_playerStats.currentExp >= PlayerLevelTable.GetXPRequiredForLevel(m_playerStats.currentLevel))
        {
            m_playerStats.currentExp -= PlayerLevelTable.GetXPRequiredForLevel(m_playerStats.currentLevel);
            m_playerStats.currentLevel++;
            Debug.Log($"🔼 Level up! New level: {m_playerStats.currentLevel}");
            m_playerStats.RecalculateStats();
        }

        m_playerStats.NotifyXPChanged();
    }
    #endregion

    #region WEAPON EXP
    public void GainWeaponXP(int amount)
    {
        m_playerStats.weaponXP += amount;
        while (m_playerStats.weaponLevel < m_playerStats.maxWeaponLevel &&
            m_playerStats.weaponXP >= GetWeaponXPRequired(m_playerStats.weaponLevel))
        {
            m_playerStats.weaponXP -= GetWeaponXPRequired(m_playerStats.weaponLevel);
            m_playerStats.weaponLevel++;
            Debug.Log($"🔼 Weapon leveled up to Lv.{m_playerStats.weaponLevel}!");
            m_playerStats.RecalculateStats();
        }
    }
    public int GetWeaponXPRequired(int level)
    {
        return 100 + (level * 25);
    }

    public float GetWeaponATK()
    {
        return WeaponStatGrowthTable.GetATK(m_playerStats.weaponLevel);
    }

    #endregion

    #region GOLD COINS
    public void AddGoldCoins(int amount)
    {
        m_playerStats.currentGoldCoins += amount;
        if (m_playerStats.currentGoldCoins < 0) m_playerStats.currentGoldCoins = 0;

        Debug.Log($"Gold Coins: {m_playerStats.currentGoldCoins}");
        // TODO: You can call a UI update here
    }
    public bool SpendGoldCoins(int amount)
    {
        if (m_playerStats.currentGoldCoins >= amount)
        {
            m_playerStats.currentGoldCoins -= amount;
            Debug.Log($"Spent {amount} coins. Remaining: {m_playerStats.currentGoldCoins}");
            return true;
        }

        Debug.Log("Not enough coins!");
        return false;
    }
    #endregion

    #region MISC

    public bool IsDead() => isDead;

    void HandleGrassInteraction() => Shader.SetGlobalVector("_Player", transform.position + Vector3.up * 0.5f);
    #endregion

    #region PAMANA EQUIPMENT
    public Dictionary<R_PamanaSlotType, R_InventoryItem> GetEquippedPamanas()
    {
        return equippedPamanaSet;
    }
    // 🧠 Holds equipped Pamanas by slot (Diwata, Lihim, Salamangkero)
    private Dictionary<R_PamanaSlotType, R_InventoryItem> equippedPamanaSet = new();

    // 🧠 Allow external read access
    public R_InventoryItem GetEquippedPamana(R_PamanaSlotType slot)
    {
        return equippedPamanaSet.TryGetValue(slot, out var item) ? item : null;
    }

    public void EquipPamana(R_InventoryItem item)
    {
        if (item == null || item.itemData == null || item.itemData.itemType != R_ItemType.Pamana)
            return;

        equippedPamanaSet[item.itemData.pamanaSlot] = item;

        ReapplyPamanaBonuses();
    }

    public void UnequipPamana(R_PamanaSlotType slot)
    {
        if (equippedPamanaSet.ContainsKey(slot))
            equippedPamanaSet[slot] = null;
        
        ReapplyPamanaBonuses();
    }

    public bool IsPamanaEquipped(R_InventoryItem item)
    {
        foreach (var kvp in equippedPamanaSet)
        {
            if (kvp.Value == item)
                return true;
        }
        return false;
    }
    private void ReapplyPamanaBonuses()
    {
        m_playerStats.ApplyPamanaBonuses(equippedPamanaSet);

        if (m_setBonus != null)
            m_setBonus.ApplySetBonuses(equippedPamanaSet);

        m_playerStats.RecalculateStats();
    }
    #endregion

    #region AGIMAT EQUIPMENT
    private R_InventoryItem equippedAgimatSlot1;
    private R_InventoryItem equippedAgimatSlot2;

    public R_InventoryItem GetEquippedAgimat(int slot)
    {
        return slot == 1 ? equippedAgimatSlot1 : equippedAgimatSlot2;
    }

    public bool IsAgimatEquipped(R_InventoryItem item)
    {
        return item != null && (item == equippedAgimatSlot1 || item == equippedAgimatSlot2);
    }

    public void EquipAgimat(R_InventoryItem item, int slotIndex)
    {
        if (item == null || item.itemData == null || item.itemData.itemType != R_ItemType.Agimat)
            return;

        if (slotIndex == 1)
            equippedAgimatSlot1 = item;
        else if (slotIndex == 2)
            equippedAgimatSlot2 = item;

        // Recalculate if needed
    }

    public void UnequipAgimat(int slotIndex)
    {
        if (slotIndex == 1)
            equippedAgimatSlot1 = null;
        else if (slotIndex == 2)
            equippedAgimatSlot2 = null;
    }

    public R_ItemRarity GetAgimatRarity(int slot)
    {
        var item = GetEquippedAgimat(slot);
        return item?.itemData?.rarity ?? R_ItemRarity.Common; // fallback to Common if null
    }
    public R_AgimatAbility GetAgimatAbility(int slot)
    {
        var item = GetEquippedAgimat(slot);
        if (item == null || item.itemData == null)
            return null;

        return slot == 1 ? item.itemData.slot1Ability : item.itemData.slot2Ability;
    }

    #endregion

}
