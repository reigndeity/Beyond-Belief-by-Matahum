using System.Collections.Generic;
using UnityEngine;

public class CircleBorderSetter : MonoBehaviour
{
    public GameObject borderPrefab;
    public int borderCount = 18;
    public float borderRadius = 50;
    public GameObject[] borderList;

    void Start()
    {
        borderList = new GameObject[borderCount];
        for (int i = 0; i < borderCount; i++)
        {
            // Step around the circle
            float angle = i * Mathf.PI * 2f / borderCount;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * borderRadius;

            // Spawn border
            GameObject border = Instantiate(borderPrefab, transform.position + pos, Quaternion.identity, transform);

            // Rotate it to face the center
            Vector3 directionToCenter = (transform.position - border.transform.position).normalized;
            border.transform.rotation = Quaternion.LookRotation(directionToCenter, Vector3.up);

            borderList[i] = border;
        }
    }
}
