using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CreateAssetMenu(menuName = "Nuno/Nuno Abilities/Stone Pillar")]
public class StonePillar_NunoAbility : Nuno_Ability
{
    public GameObject stonePillarHolderPrefab;
    public int maxPillarCount = 10;
    public float spawnInterval = 0.25f;

    public override void Activate()
    {

        float delay = 3;
        for (int i = 0; i < maxPillarCount; i++)
        {
            delay += spawnInterval;
            CoroutineRunner.Instance.RunCoroutine(SpawnStonePillar(delay));
        }
    }
    IEnumerator SpawnStonePillar(float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(stonePillarHolderPrefab, FindPlayerPosition(), Quaternion.identity, Nuno_AttackManager.Instance.transform.parent);

    }

    private Vector3 FindPlayerPosition()
    {
        Transform playerTransform = FindFirstObjectByType<Player>().transform;

        Vector3 playerPos = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);

        return playerPos;
    }
}
