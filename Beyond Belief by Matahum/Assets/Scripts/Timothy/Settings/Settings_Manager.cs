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

    public void SaveSettingsOnReturn()
    {
        Debug.Log("Saved");
        PlayerPrefs.SetFloat(cameraSensitivityPlayerPrefs, CameraSensitivityValue());
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        LoadCameraSetting();
    }

    private void Awake()
    {

    }

    private void Start()
    {
        SubscribeToListeners();

        LoadSettings();
    }

    void SubscribeToListeners()
    {
        returnButton.onClick.AddListener(SaveSettingsOnReturn);

        camSensSlider.onValueChanged.AddListener(delegate { OnCamSensSliderChanged(); });

    }

    private void Update()
    {
        
    }

    #region Camera Sensitivity Setting
    public void LoadCameraSetting()
    {
        camSensSlider.minValue = 0;
        camSensSlider.maxValue = 100;

        playerCam.mouseSensitivity = PlayerPrefs.GetFloat(cameraSensitivityPlayerPrefs, 50);
        camSensSlider.value = playerCam.mouseSensitivity / 2.5f;
        CameraSensitivityTextChange();
    }
    public void OnCamSensSliderChanged()
    {
        playerCam.mouseSensitivity = CameraSensitivityValue();
        CameraSensitivityTextChange();
    }

    public void CameraSensitivityTextChange()
    {
        camSensValue.text = camSensSlider.value.ToString("0");
    }
    public float CameraSensitivityValue()
    {
        return camSensSlider.value * 2.5f;
    }
    #endregion
}
