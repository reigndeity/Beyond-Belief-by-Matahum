using UnityEngine;

public class DuwendeVFX : MonoBehaviour
{
    public ParticleSystem duwendeAttackOne;
    public ParticleSystem duwendeAttackTwo;
    public ParticleSystem duwendeAttackThree;

    public void AttackOne()
    {
        duwendeAttackOne.Play();
    }
    public void AttackTwo()
    {
        duwendeAttackTwo.Play();
    }
        public void AttackThree()
    {
        duwendeAttackThree.Play();
    }
}
