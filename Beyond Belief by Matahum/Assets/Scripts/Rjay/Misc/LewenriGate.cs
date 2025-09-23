using UnityEngine;

public class LewenriGate : MonoBehaviour
{
    [Header("Gate Settings")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Tooltip("Rotation for when the doors are open (local euler angles).")]
    [SerializeField] private Vector3 leftDoorOpenRotation = new Vector3(0, -90, 0);
    [SerializeField] private Vector3 rightDoorOpenRotation = new Vector3(0, 90, 0);

    [Tooltip("How fast the doors rotate.")]
    [SerializeField] private float openSpeed = 2f;

    private Quaternion leftClosedRot;
    private Quaternion rightClosedRot;
    private Quaternion leftOpenRot;
    private Quaternion rightOpenRot;

    private bool isOpen = false;
    private bool isMoving = false;

    private void Awake()
    {
        if (leftDoor != null)
        {
            leftClosedRot = leftDoor.localRotation;
            leftOpenRot = Quaternion.Euler(leftDoorOpenRotation);
        }

        if (rightDoor != null)
        {
            rightClosedRot = rightDoor.localRotation;
            rightOpenRot = Quaternion.Euler(rightDoorOpenRotation);
        }
    }

    public void Open()
    {
        if (!isOpen && !isMoving)
            StartCoroutine(RotateDoors(leftOpenRot, rightOpenRot, true));

        TutorialManager.instance.tutorial_isGateOpen = true;
    }

    public void Close()
    {
        if (isOpen && !isMoving)
            StartCoroutine(RotateDoors(leftClosedRot, rightClosedRot, false));
    }

    private System.Collections.IEnumerator RotateDoors(Quaternion leftTarget, Quaternion rightTarget, bool opening)
    {
        isMoving = true;

        while ((leftDoor.localRotation != leftTarget) || (rightDoor.localRotation != rightTarget))
        {
            if (leftDoor != null)
                leftDoor.localRotation = Quaternion.Lerp(leftDoor.localRotation, leftTarget, Time.deltaTime * openSpeed);

            if (rightDoor != null)
                rightDoor.localRotation = Quaternion.Lerp(rightDoor.localRotation, rightTarget, Time.deltaTime * openSpeed);

            yield return null;
        }

        leftDoor.localRotation = leftTarget;
        rightDoor.localRotation = rightTarget;

        isOpen = opening;
        isMoving = false;
    }
}
