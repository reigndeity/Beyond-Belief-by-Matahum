using System.Collections;
using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    public float areaDuration;
    public GameObject poisonEffectObj;
    private void Start()
    {
        StartCoroutine(ScaleToSize());        
    }

    IEnumerator ScaleToSize()
    {
        Vector3 initialSize = new Vector3(0, 1, 0);

        float elapsed = 0f;
        float scaleDuration = 0.5f;
        // Scale in
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            transform.parent.localScale = Vector3.Lerp(initialSize, Vector3.one, t);
            yield return null;
        }
        yield return new WaitForSeconds(areaDuration);

        elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            transform.parent.localScale = Vector3.Lerp(Vector3.one, initialSize, t);
            yield return null;
        }

        Destroy(transform.parent.gameObject);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PoisonEffect poison = other.GetComponentInChildren<PoisonEffect>();
            if (poison == null)
            {
                Vector3 offset = new Vector3(0, 1, 0);
                poison = Instantiate(poisonEffectObj, other.transform.position + offset, Quaternion.identity, other.transform).GetComponent<PoisonEffect>();
            }

            // Refresh poison
            poison.RestartPoison();
            poison.Initialize(false);
        }
    }
}
