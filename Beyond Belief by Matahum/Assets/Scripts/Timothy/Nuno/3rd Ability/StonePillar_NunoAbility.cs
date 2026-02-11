using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Nuno/Nuno Abilities/Stone Pillar")]
public class StonePillar_NunoAbility : Nuno_Ability
{
    public GameObject stonePillarHolderPrefab;
    public int maxPillarCount = 10;
    public float spawnInterval = 0.25f;
    private Coroutine runningCoroutine;

    private List<GameObject> pillarList = new List<GameObject>();

    public override void Activate()
    {
        runningCoroutine = CoroutineRunner.Instance.RunCoroutine(SpawnAllPillars());
    }

    public override void Deactivate()
    {
        if (runningCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(runningCoroutine);
            runningCoroutine = null;

            foreach (var pillar in pillarList)
            {
                if (pillar != null)
                    Destroy(pillar);
            }

            pillarList.Clear();
        }
    }

    private IEnumerator SpawnAllPillars()
    {
        float delay = 3;
        for (int i = 0; i < maxPillarCount; i++)
        {
            yield return new WaitForSeconds(delay);
            GameObject pillar = Instantiate(stonePillarHolderPrefab, FindPlayerPosition(), Quaternion.identity, Nuno_AttackManager.Instance.transform.parent);
            pillarList.Add(pillar);
            delay = spawnInterval; // after first spawn, use spawnInterval
        }
    }

    private Vector3 FindPlayerPosition()
    {
        Transform playerTransform = FindFirstObjectByType<Player>().transform;

        Vector3 playerPos = new Vector3(playerTransform.position.x, -0.65f, playerTransform.position.z);

        return playerPos;
    }
}
