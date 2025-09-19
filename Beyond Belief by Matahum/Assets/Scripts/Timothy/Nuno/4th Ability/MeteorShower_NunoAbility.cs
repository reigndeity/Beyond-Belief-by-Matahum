using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nuno/Nuno Abilities/Meteor Shower")]
public class MeteorShower_NunoAbility : Nuno_Ability
{
    public GameObject meteorShowerPrefab;
    [HideInInspector] public Transform center;
    public float radius = 5f;
    public int spawnCount = 10;
    public float minDistanceBetween = 1f;
    public float meteorLifetime;
    public float startMovingDelay = 3f;

    [HideInInspector] public Vector3[] areaToSpawn;

    private Coroutine runningCoroutine;
    private List<Coroutine> meteorCoroutines = new List<Coroutine>();
    private List<GameObject> spawnedMeteors = new List<GameObject>();

    public override void Activate()
    {
        center = FindFirstObjectByType<Nuno_AttackManager>().transform;
        GenerateSpawnPositions();

        // Run one master coroutine that handles the whole sequence
        runningCoroutine = CoroutineRunner.Instance.RunCoroutine(SpawnAllMeteors());
    }

    public override void Deactivate()
    {
        if (runningCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        foreach (var c in meteorCoroutines)
        {
            if (c != null)
                CoroutineRunner.Instance.StopCoroutine(c);
        }
        meteorCoroutines.Clear();

        foreach (var m in spawnedMeteors)
        {
            if (m != null) Destroy(m);
        }
        spawnedMeteors.Clear();
    }

    private IEnumerator SpawnAllMeteors()
    {
        float delayBetween = 0.1f;

        for (int i = 0; i < areaToSpawn.Length; i++)
        {
            yield return new WaitForSeconds(delayBetween);
            CoroutineRunner.Instance.RunCoroutine(SpawnMeteor(areaToSpawn[i]));
        }
    }

    private IEnumerator SpawnMeteor(Vector3 position)
    {
        yield return new WaitForSeconds(2f);

        GameObject meteorObj = Instantiate(
            meteorShowerPrefab,
            position,
            Quaternion.identity,
            Nuno_AttackManager.Instance.transform.parent
        );
        spawnedMeteors.Add(meteorObj);

        MeteorShower_Holder meteorHolder = meteorObj.GetComponent<MeteorShower_Holder>();
        meteorHolder.transform.position = position;

        Vector3 initialSize = new Vector3(0, 1, 0);
        float elapsed = 0f;
        float scaleDuration = 1;

        // Scale in smoothly
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            meteorHolder.transform.localScale = Vector3.Lerp(initialSize, Vector3.one, t);
            yield return null;
        }

        yield return new WaitForSeconds(startMovingDelay);

        // Start falling
        meteorHolder.meteorObj.GetComponent<MeteorShower_MeteorBullet>().startFalling = true;

        Destroy(meteorObj, meteorLifetime);
    }

    private void GenerateSpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 newPos;
            int safety = 0;

            do
            {
                newPos = RandomPosition(center);
                safety++;
                if (safety > 100) break;
            }
            while (!IsValidPosition(newPos, positions));

            positions.Add(newPos);
        }

        areaToSpawn = positions.ToArray();
    }

    private Vector3 RandomPosition(Transform center)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return center.position + new Vector3(randomCircle.x, -0.65f, randomCircle.y);
    }

    private bool IsValidPosition(Vector3 newPos, List<Vector3> existing)
    {
        foreach (var pos in existing)
        {
            if (Vector3.Distance(newPos, pos) < minDistanceBetween)
                return false;
        }
        return true;
    }
}
