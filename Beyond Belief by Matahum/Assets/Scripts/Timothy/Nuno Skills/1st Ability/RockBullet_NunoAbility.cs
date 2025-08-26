using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(menuName = "Nuno/Nuno Abilities/Rock Bullet")]
public class RockBullet_NunoAbility : Nuno_Ability
{
    public GameObject bulletPrefab;
    [HideInInspector] public Transform[] bulletPosition;

    public override void Activate()
    {
        bulletPosition = Nuno_AttackManager.Instance.bulletPosition;

        float delay = 0;
        foreach(Transform transform in bulletPosition)
        {
            delay += 0.5f;
            CoroutineRunner.Instance.RunCoroutine(DelaySpawn(transform, delay));
        }
    }
    IEnumerator DelaySpawn(Transform transform, float delay)
    {
        yield return new WaitForSeconds(delay);
        CoroutineRunner.Instance.RunCoroutine(SpawnBullets(transform));
    }

    IEnumerator SpawnBullets(Transform position)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, Nuno_AttackManager.Instance.bulletHolder);
        bulletObj.transform.position = position.position;
        bulletObj.transform.localScale = Vector3.zero;
        RockBullet_Bullet bullet = bulletObj.GetComponent<RockBullet_Bullet>();

        float elapsed = 0f;
        float scaleDuration = 0.25f;

        // Scale in
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            bullet.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }

        yield return new WaitForSeconds(3);
        bullet.lastPlayerPosition = GetPlayerLastPosition(position);
        bullet.canShoot = true;
        bullet.transform.SetParent(null);
        Destroy(bullet.gameObject, 8);
    }

    private Vector3 GetPlayerLastPosition(Transform position)
    {
        Vector3 playerPos = FindFirstObjectByType<Player>().transform.position;
        Vector3 offset = new Vector3(0, 1f, 0);
        Vector3 targetPos = ((playerPos + offset) - position.position).normalized;
        
        return targetPos;
    }
}
