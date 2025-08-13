using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class DamagePopUpGenerator : MonoBehaviour
{
    public static DamagePopUpGenerator instance;
    public GameObject damagePopupPrefab;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    public void CreatePopUp(Vector3 position, string text, Color color)
    {
        var popup = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        temp.text = text;
        temp.faceColor = color;
        Destroy(popup, 1f);
    }
}
