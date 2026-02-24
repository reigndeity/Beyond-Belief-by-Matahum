using UnityEngine;

public class DamageTakenGivenCompiler : MonoBehaviour
{
    public float CurrentDamageDealt;
    public float CurrentDamageTaken;

    public float GetDamageDealt(float DamageDealt)
    {
        CurrentDamageDealt += DamageDealt;

        return DamageDealt;
    }

    public float GetDamageTaken(float DamageTaken)
    {
        CurrentDamageTaken += DamageTaken;

        return DamageTaken;
    }

    public void ResetValues()
    {
        CurrentDamageDealt = 0;
        CurrentDamageTaken = 0;
    }
}
