using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private UI_Game m_uiGame;

    void Awake()
    {
        m_uiGame = FindFirstObjectByType<UI_Game>();
    }
}
