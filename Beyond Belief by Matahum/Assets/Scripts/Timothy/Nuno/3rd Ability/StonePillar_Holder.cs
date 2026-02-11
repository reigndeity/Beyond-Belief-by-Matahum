using UnityEngine;
using System.Collections;

public class StonePillar_Holder : MonoBehaviour
{
    public GameObject stonePillarHolderObj;
    public float pillarSpeed = 1f;
    public float duration = 2f;

    private void Start()
    {
        StartCoroutine(SpawnPillar());
    }

    IEnumerator SpawnPillar()
    {
        yield return new WaitForSeconds(pillarSpeed);
        stonePillarHolderObj.SetActive(true);
        yield return new WaitForSeconds(duration);
        stonePillarHolderObj.SetActive(false);
        Destroy(transform.gameObject);
    }
}
