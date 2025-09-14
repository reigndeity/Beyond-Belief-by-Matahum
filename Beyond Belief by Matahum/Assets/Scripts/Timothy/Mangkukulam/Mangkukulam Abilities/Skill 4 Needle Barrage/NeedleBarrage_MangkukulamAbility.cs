using UnityEngine;
using System.Collections;
using DissolveExample;

[CreateAssetMenu(menuName = "Mangkukulam/Mangkukulam Abilities/Needle Barrage")]

public class NeedleBarrage : Mangkukulam_Ability
{
    public Coroutine currentCoroutine;
    public float cooldown;

    public GameObject needleBarrageHolderPrefab;
    public override Coroutine AbilityActivate() => currentCoroutine;
    public override float Cooldown() => cooldown;
    public override void Activate(GameObject user)
    {
        CoroutineRunner.Instance.RunCoroutine(StartSpinning(user));
    }
    IEnumerator StartSpinning(GameObject user)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 offset = new Vector3(0, 3, 0);

        // Pick a random Y rotation (0–360°)
        float randomize = Random.Range(0f, 360f);
        Quaternion randomizeRotation = Quaternion.Euler(0, randomize, 0);

        // Spawn with random rotation
        GameObject needleHolderPrefab = Instantiate(
            needleBarrageHolderPrefab, //prefab
            user.transform.parent.position + offset, //position
            randomizeRotation, //rotation
            user.transform.parent //parent
        );

        NeedleBarrageHolder holderScript = needleHolderPrefab.GetComponent<NeedleBarrageHolder>();
    }
}
