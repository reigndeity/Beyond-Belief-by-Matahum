using UnityEngine;

public class UI_Billboarding : MonoBehaviour
{
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }
    void Update()
    {
        transform.forward = cam.transform.forward;
    }
}
