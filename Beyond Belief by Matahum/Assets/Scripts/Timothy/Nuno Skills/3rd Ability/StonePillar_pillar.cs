using System.Collections;
using UnityEngine;

public class StonePillar_Pillar : MonoBehaviour
{
    public float pillarDamage;
    public float pillarHeight;
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
        float riseDuration = 0.25f;
        // Scale in
        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / riseDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        Debug.Log("Starts Rising");
        StartCoroutine(StartFalling(endPos, startPos));
    }
    IEnumerator StartFalling(Vector3 topPos, Vector3 bottomPos)
    {
        Vector3 pillarHeightPos = new Vector3(0, pillarHeight, 0);
        float elapsed = 0f;
        float riseDuration = 0.5f;
        // Scale in
        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / riseDuration;
            transform.position = Vector3.Lerp(topPos, bottomPos, t);
            yield return null;
        }
        Debug.Log("Starts Falling");
        Destroy(transform.parent.gameObject);
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
