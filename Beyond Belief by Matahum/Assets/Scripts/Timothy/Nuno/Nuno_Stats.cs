using UnityEngine;

public class Nuno_Stats : MonoBehaviour
{
    [Header("Computed Enemy Stats")]
    public float n_currentHealth;
    public float n_maxHealth;
    public float n_attack;
    public float n_defense;
    public float n_criticalRate;    // %
    public float n_criticalDamage;  // %

    private void Start()
    {
        n_currentHealth = Mathf.Clamp(n_currentHealth <= 0 ? n_maxHealth : n_currentHealth, 0f, n_maxHealth);
    }
    /// <summary>Returns true if the enemy died.</summary>
    public bool ApplyDamage(float amount)
    {
        n_currentHealth = Mathf.Clamp(n_currentHealth - amount, 0f, n_maxHealth);
        return n_currentHealth <= 0f;
    }
}
