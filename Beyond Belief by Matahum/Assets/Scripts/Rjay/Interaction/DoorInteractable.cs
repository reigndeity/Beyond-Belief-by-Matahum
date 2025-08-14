using System.Collections;
using UnityEngine;

public class DoorInteractable : Interactable
{
    [Header("Door Setup")]
    [Tooltip("Parent you rotate (the hinge object you made).")]
    public Transform doorPivot;

    [Tooltip("Closed Y angle (local). Usually 0).")]
    public float closedAngleY = 0f;

    [Tooltip("Open Y angle (local). 90 or -90 depending on hinge side.")]
    public float openAngleY = 90f;

    [Header("Behavior")]
    public float speedDegPerSec = 220f;
    public bool startOpen = false;
    public bool autoClose = false;
    public float autoCloseDelay = 2.0f;

    [Header("Optional SFX")]
    public AudioSource audioSource;
    public AudioClip openSfx;
    public AudioClip closeSfx;

    public bool isOpen;
    bool isMoving;
    float targetY;
    Coroutine moveCo;

    void Awake()
    {
        isOpen = startOpen;
        targetY = isOpen ? openAngleY : closedAngleY;
        if (doorPivot != null)
            doorPivot.localRotation = Quaternion.Euler(0f, targetY, 0f);
    }

    public override void OnInteract()
    {
        // Your Interactable handles input/range; this just toggles. :contentReference[oaicite:0]{index=0}
        if (doorPivot == null || isMoving) return;

        ToggleDoor();

        if (autoClose && isOpen)
        {
            StopAllCoroutines();
            moveCo = StartCoroutine(AutoCloseAfter(autoCloseDelay));
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        targetY = isOpen ? openAngleY : closedAngleY;

        if (moveCo != null) StopCoroutine(moveCo);
        moveCo = StartCoroutine(RotateTo(targetY));

        if (audioSource != null)
        {
            var clip = isOpen ? openSfx : closeSfx;
            if (clip) audioSource.PlayOneShot(clip);
        }
    }

    System.Collections.IEnumerator AutoCloseAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isOpen && !isMoving) ToggleDoor();
    }

    IEnumerator RotateTo(float targetAngleY)
    {
        isMoving = true;

        while (true)
        {
            float currentY = doorPivot.localEulerAngles.y;
            float delta = Mathf.DeltaAngle(currentY, targetAngleY);
            if (Mathf.Abs(delta) < 0.1f)
                break;

            float step = Mathf.Sign(delta) * speedDegPerSec * Time.deltaTime;
            // clamp step so we don't overshoot
            if (Mathf.Abs(step) > Mathf.Abs(delta)) step = delta;

            doorPivot.localRotation = Quaternion.Euler(0f, currentY + step, 0f);
            yield return null;
        }

        doorPivot.localRotation = Quaternion.Euler(0f, targetAngleY, 0f);
        isMoving = false;
    }
}
