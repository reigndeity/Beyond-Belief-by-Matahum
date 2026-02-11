using System.Collections;
using UnityEngine;

public class UltimateSkillVFX : MonoBehaviour
{
    [Header("Spin Settings")]
    public float rotationSpeed = 90f;      // Degrees per second
    public int spinDirection = 1;          // 1 for clockwise, -1 for counterclockwise

    private Coroutine spinRoutine;

    void Start()
    {
        StartSpin();
    }

    public void StartSpin()
    {
        if (spinRoutine != null)
            StopCoroutine(spinRoutine);

        spinRoutine = StartCoroutine(SpinForever());
    }

    public void StopSpin()
    {
        if (spinRoutine != null)
        {
            StopCoroutine(spinRoutine);
            spinRoutine = null;
        }
    }

    private IEnumerator SpinForever()
    {
        while (true)
        {
            float rotationThisFrame = rotationSpeed * spinDirection * Time.deltaTime;
            transform.Rotate(Vector3.up * rotationThisFrame, Space.World);
            yield return null;
        }
    }
}
