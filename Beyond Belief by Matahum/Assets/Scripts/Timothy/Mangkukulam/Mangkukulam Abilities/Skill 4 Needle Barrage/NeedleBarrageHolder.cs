using System.Collections;
using UnityEngine;

public class NeedleBarrageHolder : MonoBehaviour
{
    [Header("Needle Setup")]
    public GameObject needlePrefab;
    public int needleCount = 8;
    public float radius = 3f;

    [Header("Spin Settings")]
    public float startSpinSpeed = 720f;  // initial fast spin
    public float endSpinSpeed = 60f;     // final slow spin before shooting
    public float spinDuration = 3f;      // how long to spin before shooting

    [Header("Shoot Settings")]
    public float needleSpeed = 15f;

    private NeedleScript[] needles;
    public bool spinning = false;
    private float spinTimer;
    private float currentSpinSpeed;

    void Start()
    {
        spinTimer = spinDuration;
        currentSpinSpeed = startSpinSpeed;

        // Create the needles in a circle
        needles = new NeedleScript[needleCount];
        for (int i = 0; i < needleCount; i++)
        {
            float angle = i * Mathf.PI * 2f / needleCount;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            needles[i] = Instantiate(needlePrefab, transform.position + pos, Quaternion.identity, transform).GetComponent<NeedleScript>();
            needles[i].transform.parent = transform; // keep them attached during spin
        }
    }

    void Update()
    {
        if (spinning)
        {
            spinTimer -= Time.deltaTime;
            float t = 1f - (spinTimer / spinDuration);
            currentSpinSpeed = Mathf.Lerp(startSpinSpeed, endSpinSpeed, t);

            // Spin holder freely
            transform.Rotate(Vector3.up, currentSpinSpeed * Time.deltaTime);

            if (spinTimer <= 0f)
            {
                spinning = false; // rotation freezes at last random angle

                // Launch needles
                float delay = 0;
                for (int i = 0; i < needles.Length; i++)
                {
                    delay += 1f / needles.Length;
                    CoroutineRunner.Instance.RunCoroutine(DelaySpawn(needles[i], delay));
                }
            }
        }

        // Destroy this holder once all needles are gone
        if (!spinning && AllNeedlesDestroyed())
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DelaySpawn(NeedleScript needle, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartShooting(needle);
    }

    void StartShooting(NeedleScript needle)
    {
        if (needle == null) return;

        needle.lastPlayerPosition = GetPlayerLastPosition(needle.transform);
        needle.speed = needleSpeed;
        needle.canShoot = true;
    }

    private Vector3 GetPlayerLastPosition(Transform position)
    {
        Vector3 playerPos = FindFirstObjectByType<Player>().transform.position;
        Vector3 offset = new Vector3(0, 1f, 0);
        Vector3 targetPos = ((playerPos + offset) - position.position).normalized;

        return targetPos;
    }

    private bool AllNeedlesDestroyed()
    {
        foreach (var needle in needles)
        {
            if (needle != null) return false;
        }
        return true;
    }

    // 🔴 Draw Gizmo for radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        // Optional: show needle spawn points as little spheres
        if (needleCount > 0)
        {
            for (int i = 0; i < needleCount; i++)
            {
                float angle = i * Mathf.PI * 2f / needleCount;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                Gizmos.DrawSphere(transform.position + pos, 0.1f);
            }
        }
    }
}
