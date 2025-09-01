using System.Collections;
using UnityEngine;

public class StonePillar_Pillar : MonoBehaviour
{
    public float pillarDamage;
    public float pillarHeight;
    public float risingDuration = 0.25f;
    public float fallingDuration = 0.5f;
    private bool dealtDamageAlready = false;

    private void Start()
    {
        StartCoroutine(StartRising());
    }
    public IEnumerator StartRising()
    {
        yield return new WaitForSeconds(1f);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * pillarHeight;

        float elapsed = 0f;
        // Scale in
        while (elapsed < risingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / risingDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        Debug.Log("Starts Rising");
        StartCoroutine(StartFalling(endPos, startPos));
    }
    IEnumerator StartFalling(Vector3 topPos, Vector3 bottomPos)
    {
        yield return new WaitForSeconds(1.25f);
        float elapsed = 0f;
        // Scale in
        while (elapsed < fallingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallingDuration;
            transform.position = Vector3.Lerp(topPos, bottomPos, t);
            yield return null;
        }
        Debug.Log("Starts Falling");
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null && !dealtDamageAlready)
        {
            dealtDamageAlready = true;
            player.TakeDamage(pillarDamage);
        }
    }
}
