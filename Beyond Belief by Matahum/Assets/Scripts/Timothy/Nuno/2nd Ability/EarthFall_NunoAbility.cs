using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nuno/Nuno Abilities/Earth Fall")]
public class EarthFall_NunoAbility : Nuno_Ability
{
    public GameObject earthFallHolderPrefab;
    public float earthFallObjLifetime;
    private Coroutine runningCoroutine;
    private GameObject spawnedPrefab;

    public override void Activate()
    {
        runningCoroutine = CoroutineRunner.Instance.RunCoroutine(SpawnEarthFall());
    }

    public override void Deactivate()
    {
        if (runningCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(runningCoroutine);
            runningCoroutine = null;

            if(spawnedPrefab != null)
                Destroy(spawnedPrefab); 
        }
    }

    IEnumerator SpawnEarthFall()
    {
        yield return new WaitForSeconds(2.5f);
        spawnedPrefab = Instantiate(earthFallHolderPrefab, FindPlayerPosition(), Quaternion.identity, Nuno_AttackManager.Instance.transform.parent);
        EarthFall_Holder earthFallHolder = spawnedPrefab.GetComponent<EarthFall_Holder>();
        Vector3 initialSize = new Vector3(0, 1, 0);

        float elapsed = 0f;
        float scaleDuration = 3;
        // Scale in
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            spawnedPrefab.transform.localScale = Vector3.Lerp(initialSize, Vector3.one, t);
            spawnedPrefab.transform.position = FindPlayerPosition();
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        earthFallHolder.earthFallPrefab.GetComponent<EarthFall_Bullet>().startFalling = true;
        Destroy(spawnedPrefab, earthFallObjLifetime);
    }

    private Vector3 FindPlayerPosition()
    {
        Transform playerTransform = FindFirstObjectByType<Player>().transform;

        Vector3 playerPos = new Vector3(playerTransform.position.x, -0.65f, playerTransform.position.z);

        return playerPos;
    }
}
