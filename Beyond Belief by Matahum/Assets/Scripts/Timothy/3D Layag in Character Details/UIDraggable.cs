using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Transform targetObject;     // The 3D object to rotate
    public float rotationSpeed = 5f;   // How sensitive the drag is
    public bool resetOnRelease = true; // Should the object snap back when released?

    private bool isDragging = false;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    private void Start()
    {
        if (targetObject != null)
        {
            initialRotation = targetObject.rotation;
            targetRotation = initialRotation;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        if (resetOnRelease)
        {
            // smoothly return to the starting rotation
            targetRotation = initialRotation;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && targetObject != null)
        {
            float deltaX = eventData.delta.x;
            targetObject.Rotate(Vector3.up, -deltaX * rotationSpeed, Space.World);

            if (resetOnRelease == false)
            {
                // keep current rotation as "resting"
                targetRotation = targetObject.rotation;
            }
        }
    }

    private void Update()
    {
        if (!isDragging && resetOnRelease && targetObject != null)
        {
            // Smoothly interpolate back to initial rotation
            targetObject.rotation = Quaternion.Slerp(targetObject.rotation, targetRotation, Time.unscaledDeltaTime * 5f);
        }
    }
}

