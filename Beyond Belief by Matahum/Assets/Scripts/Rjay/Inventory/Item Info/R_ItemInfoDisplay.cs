using UnityEngine;

public abstract class R_ItemInfoDisplay : MonoBehaviour
{
    public abstract void Show(R_ItemData itemData);
    public abstract void Hide();
}