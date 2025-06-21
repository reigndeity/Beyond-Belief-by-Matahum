using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ProximityTrigger : MonoBehaviour
{
    public UnityEvent onPlayerNearby;
    public Button buttonObject;
    private GameObject playerCanvas;

    private void Start()
    {
        playerCanvas = GameObject.Find("Interact Buttons Holder");
        SetUpButtion();
    }

    public void PlayerEntersCollision()
    {
        buttonObject.transform.SetParent(playerCanvas.transform, false);
        buttonObject.GetComponent<RectTransform>().localScale = Vector3.one;
        buttonObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
        buttonObject.gameObject.SetActive(true);
    }

    public void PlayerExitsCollision()
    {
        buttonObject.transform.SetParent(this.transform, false);
        buttonObject.gameObject.SetActive(false);
    }

    public void Interact()
    {
        onPlayerNearby?.Invoke();
    }

    void SetUpButtion()
    {
        TextMeshProUGUI btnText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
        btnText.text = $"{gameObject.name}";
        buttonObject.name = $"{gameObject.name} Button";
        buttonObject.gameObject.SetActive(false);
        buttonObject.onClick.AddListener(Interact);
    }
}
