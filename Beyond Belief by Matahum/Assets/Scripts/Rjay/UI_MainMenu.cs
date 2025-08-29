using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(OnClickStart);
    }
    private void OnClickStart()
    {
        Loader.Load(3);
    }
}
