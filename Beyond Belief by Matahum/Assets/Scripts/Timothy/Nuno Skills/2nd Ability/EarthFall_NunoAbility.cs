using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CreateAssetMenu(menuName = "Nuno/Nuno Abilities/Earth Fall")]
public class EarthFall_NunoAbility : Nuno_Ability
{
    public GameObject earthFallHolderPrefab;
    public float earthFallObjLifetime;

    [HideInInspector] public Vector3[] areaToSpawn; // Store positions instead of transforms
    public float startMovingDelay = 3f;

    public override void Activate()
    {
        CoroutineRunner.Instance.RunCoroutine(SpawnEarthFall());
    }

    IEnumerator SpawnEarthFall()
    {
        //yield return new WaitForSeconds(2.5f);
        GameObject earthFallObj = Instantiate(earthFallHolderPrefab, FindPlayerPosition(), Quaternion.identity, Nuno_AttackManager.Instance.transform.parent);
        EarthFall_Holder earthFallHolder = earthFallObj.GetComponent<EarthFall_Holder>();
        Vector3 initialSize = new Vector3(0, 1, 0);

        float elapsed = 0f;
        float scaleDuration = 3;
        // Scale in
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            earthFallObj.transform.localScale = Vector3.Lerp(initialSize, Vector3.one, t);
            earthFallObj.transform.position = FindPlayerPosition();
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        earthFallHolder.earthFallPrefab.GetComponent<EarthFall_Bullet>().startFalling = true;
        Destroy(earthFallObj, earthFallObjLifetime);
    }

    private Vector3 FindPlayerPosition()
    {
        Transform playerTransform = FindFirstObjectByType<Player>().transform;

        Vector3 playerPos = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);

        return playerPos;
    }
}
