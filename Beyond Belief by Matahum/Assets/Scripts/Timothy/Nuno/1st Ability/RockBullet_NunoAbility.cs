using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Nuno/Nuno Abilities/Rock Bullet")]
public class RockBullet_NunoAbility : Nuno_Ability
{
    public RockBulletHolder holder;
    private RockBulletHolder currentHolder;
    public GameObject bulletPrefab;
    //[HideInInspector] public Transform[] bulletPosition;

    public override void Activate()
    {
        /*bulletPosition = holder.bulletPosition;

        float delay = 0;
        foreach(Transform transform in bulletPosition)
        {
            delay += 0.4f;
            CoroutineRunner.Instance.RunCoroutine(DelaySpawn(transform, delay));
        }*/

        Transform nuno = Nuno_AttackManager.Instance.transform;
        currentHolder = Instantiate(holder, nuno.position, nuno.rotation, nuno);
        currentHolder.bulletPrefab = bulletPrefab;
        currentHolder.StartSpawning();
    }

    public override void Deactivate()
    {
        if(currentHolder != null) 
        {
            currentHolder.StopAllCoroutines();
            Destroy(currentHolder.gameObject);
            currentHolder = null;
        }
            
    }
    /*IEnumerator DelaySpawn(Transform transform, float delay)
    {
        yield return new WaitForSeconds(delay);
        CoroutineRunner.Instance.RunCoroutine(SpawnBullets(transform));
    }

    IEnumerator SpawnBullets(Transform position)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, holder.bulletHolder);
        bulletObj.transform.position = position.position;
        bulletObj.transform.localScale = Vector3.zero;
        RockBullet_Bullet bullet = bulletObj.GetComponent<RockBullet_Bullet>();

        float elapsed = 0f;
        float scaleDuration = 2f;

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
    }*/
}
