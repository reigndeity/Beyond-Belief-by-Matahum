using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nuno/Nuno Abilities/Meteor Shower")]
public class MeteorShower_NunoAbility : Nuno_Ability
{
    public GameObject meteorShowerPrefab;
    [HideInInspector]public Transform center; // The middle point to spawn around
    public float radius = 5f; // Radius around the center
    public int spawnCount = 10; // How many meteors to spawn
    public float minDistanceBetween = 1f; // Prevent overlapping
    public float meteorLifetime;

    [HideInInspector] public Vector3[] areaToSpawn; // Store positions instead of transforms
    public float startMovingDelay = 3f;

    public override void Activate()
    {
        center = FindFirstObjectByType<Nuno_AttackManager>().transform;
        // Generate random positions each time we activate
        GenerateSpawnPositions();

        float delay = 0;
        foreach (Vector3 pos in areaToSpawn)
        {
            delay += 0.1f;
            CoroutineRunner.Instance.RunCoroutine(DelaySpawn(pos, delay));
        }
    }

    IEnumerator DelaySpawn(Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);
        CoroutineRunner.Instance.RunCoroutine(SpawnMeteor(pos));
    }

    IEnumerator SpawnMeteor(Vector3 position)
    {
        yield return new WaitForSeconds(2f);
        GameObject meteorObj = Instantiate(meteorShowerPrefab, position, Quaternion.identity, Nuno_AttackManager.Instance.transform.parent);
        MeteorShower_Holder meteorHolder = meteorObj.GetComponent<MeteorShower_Holder>();
        meteorHolder.transform.position = position;
        Vector3 initialSize = new Vector3(0, 1, 0);

        float elapsed = 0f;
        float scaleDuration = 1;
        // Scale in
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            meteorHolder.transform.localScale = Vector3.Lerp(initialSize, Vector3.one, t);
            yield return null;
        }

        yield return new WaitForSeconds(startMovingDelay);
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

            // Try finding a non-overlapping position
            do
            {
                newPos = RandomPosition(center);
                safety++;
                if (safety > 100) break; // Prevent infinite loops if radius is too small
            }
            while (!IsValidPosition(newPos, positions));

            positions.Add(newPos);
        }

        areaToSpawn = positions.ToArray();
    }

    private Vector3 RandomPosition(Transform center)
    {
        // Pick a random point inside a circle (2D) or sphere (3D)
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 targetPos = center.position + new Vector3(randomCircle.x, -0.65f, randomCircle.y);

        return targetPos;
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
