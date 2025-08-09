using UnityEngine;
public class DevSaveHotkeys : MonoBehaviour
{
    [SerializeField] string slot = "Slot_01";
    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) await SaveManager.Instance.SaveAsync(slot);
        if (Input.GetKeyDown(KeyCode.F9)) await SaveManager.Instance.LoadAsync(slot);
    }
}
