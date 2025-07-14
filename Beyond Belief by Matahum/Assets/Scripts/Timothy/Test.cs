using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ItemActionController.Instance.RemoveSpecificItem("Garlic_Material", 2);
    }
}
