using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings_Manager : MonoBehaviour
{
    [Header("References")]
    public PlayerCamera playerCam;

    [Header("Camera Sensitivity")]
    public TextMeshProUGUI camSensValueText;
    public Slider camSensSlider;
    private const string cameraSensitivityPlayerPrefs = "Camera_Sensitivity";

    [Header("Audio Settings")]
    public TextMeshProUGUI sfxValueText;
    public Slider sfxSlider;
    public TextMeshProUGUI bgmValueText;
    public Slider bgmSlider;
    public TextMeshProUGUI ambienceValueText;
    public Slider ambienceSlider;
    private const string audioSFXPlayerPrefs = "Audio_SFX";
    private const string audioBGMPlayerPrefs = "Audio_BGM";
    private const string audioAmbiencePlayerPrefs = "Audio_Ambience";

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

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);

        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);

        if (ambienceSlider != null)
            ambienceSlider.onValueChanged.AddListener(OnAmbienceSliderChanged);
    }

    #region Save & Load
    public void SaveSettingsOnReturn()
    {
        PlayerPrefs.SetFloat(cameraSensitivityPlayerPrefs, CameraSensitivityValue());
        PlayerPrefs.SetFloat(audioSFXPlayerPrefs, sfxSlider.value);
        PlayerPrefs.SetFloat(audioBGMPlayerPrefs, bgmSlider.value);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        LoadCameraSetting();
        LoadAudioSetting();
    }
    #endregion

    #region Camera Sensitivity
    void LoadCameraSetting()
    {
        camSensSlider.minValue = 0.01f;
        camSensSlider.maxValue = 1;

        float savedSensitivity = PlayerPrefs.GetFloat(cameraSensitivityPlayerPrefs, 125f);
        camSensSlider.value = savedSensitivity / 250f;

        if (playerCam != null)
            playerCam.mouseSensitivity = savedSensitivity;

        camSensValueText.text = camSensSlider.value.ToString("F2");
    }

    void OnCamSensSliderChanged()
    {
        if (playerCam != null)
            playerCam.mouseSensitivity = CameraSensitivityValue();

        camSensValueText.text = camSensSlider.value.ToString("F2");
    }

    float CameraSensitivityValue() => camSensSlider.value * 250f;
    #endregion

    #region Audio
    void LoadAudioSetting()
    {
        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 1f;
        bgmSlider.minValue = 0f;
        bgmSlider.maxValue = 1f;
        ambienceSlider.minValue = 0f;
        ambienceSlider.maxValue = 1f;

        float savedSFX = PlayerPrefs.GetFloat(audioSFXPlayerPrefs, 1);
        float savedBGM = PlayerPrefs.GetFloat(audioBGMPlayerPrefs, 1f);
        float savedAmbience = PlayerPrefs.GetFloat(audioAmbiencePlayerPrefs, 1f);

        sfxSlider.value = savedSFX;
        bgmSlider.value = savedBGM;
        ambienceSlider.value = savedAmbience;

        AudioManager.instance.SetSFXVolume(savedSFX);
        AudioManager.instance.SetBGMVolume(savedBGM);
        AudioManager.instance.SetAmbienceVolume(savedAmbience);

        UpdateAudioText();
    }

    void OnSFXSliderChanged(float value)
    {
        AudioManager.instance.SetSFXVolume(sfxSlider.value);
        UpdateAudioText();
    }

    void OnBGMSliderChanged(float value)
    {
        AudioManager.instance.SetBGMVolume(bgmSlider.value);
        UpdateAudioText();
    }

    void OnAmbienceSliderChanged(float value)
    {
        AudioManager.instance.SetAmbienceVolume(ambienceSlider.value);
        UpdateAudioText();
    }

    void UpdateAudioText()
    {
        sfxValueText.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();
        bgmValueText.text = Mathf.RoundToInt(bgmSlider.value * 100).ToString();
        ambienceValueText.text = Mathf.RoundToInt(ambienceSlider.value * 100).ToString();
    }

    #endregion
}
