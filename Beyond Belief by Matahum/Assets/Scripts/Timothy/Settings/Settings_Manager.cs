using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings_Manager : MonoBehaviour
{
    [Header("References")]
    public PlayerCamera playerCam;

    [Header("Camera Sensitivity")]
    private const string cameraSensitivityPlayerPrefs = "Camera_Sensitivity";
    public TextMeshProUGUI camSensValue;
    public Slider camSensSlider;

    [Header("Buttons")]
    public Button returnButton;

    private void Start()
    {
        SubscribeToListeners();
        LoadSettings();
    }

    void SubscribeToListeners()
    {
        if (returnButton != null)
            returnButton.onClick.AddListener(SaveSettingsOnReturn);

        if (camSensSlider != null)
            camSensSlider.onValueChanged.AddListener(delegate { OnCamSensSliderChanged(); });
    }

    public void SaveSettingsOnReturn()
    {
        PlayerPrefs.SetFloat(cameraSensitivityPlayerPrefs, CameraSensitivityValue());
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        LoadCameraSetting();
    }

    #region Camera Sensitivity Setting
    public void LoadCameraSetting()
    {
        camSensSlider.minValue = 0.01f;
        camSensSlider.maxValue = 1;

        float savedSensitivity = PlayerPrefs.GetFloat(cameraSensitivityPlayerPrefs, 125);

        if (playerCam != null)
            playerCam.mouseSensitivity = savedSensitivity;

        camSensSlider.value = savedSensitivity / 250f;

        CameraSensitivityTextChange();
    }

    public void OnCamSensSliderChanged()
    {
        if (playerCam != null)
            playerCam.mouseSensitivity = CameraSensitivityValue();

        CameraSensitivityTextChange();
    }

    public void CameraSensitivityTextChange()
    {
        camSensValue.text = camSensSlider.value.ToString("F2");
    }

    public float CameraSensitivityValue()
    {
        return camSensSlider.value * 250;
    }
    #endregion
}
