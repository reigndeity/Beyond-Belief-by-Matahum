using System.Collections.Generic;
using UnityEngine;

public class RockBullet_ObjectPool : MonoBehaviour
{
    public GameObject rockBulletPrefab;
    public Transform bulletHolder;

    public GameObject GetBullet()
    {
        GameObject obj = Instantiate(rockBulletPrefab, bulletHolder);

        // always make sure bullet knows its pool
        obj.GetComponent<RockBullet_Bullet>().pool = this;
        return obj;
    }
}
