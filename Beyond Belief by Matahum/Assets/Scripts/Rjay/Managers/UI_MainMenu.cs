using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] Button newGameButton;
    [SerializeField] Button continueGameButton;
    [SerializeField] Button closeGameButton;

    [Header("New Game Confirmation")]
    [SerializeField] GameObject newGameConfirmationPanel;
    [SerializeField] Button noNewGameButton;
    [SerializeField] Button yesNewGameButton;

    [Header("Close Game Confirmation")]
    [SerializeField] GameObject closeGameConfirmationPanel;
    [SerializeField] Button noCloseGameButton;
    [SerializeField] Button yesCloseGameButton;

    [Header("Settings")]
    [SerializeField] GameObject settingsPanel;
    [SerializeField] Button settingsButton;
    [SerializeField] Button returnButton;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject displayAndGraphicsPanel;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private Button audioButton;
    [SerializeField] private Button displayAndGraphicsButton;
    [SerializeField] private Button controlButton;

    private string savePath;

    void Awake()
    {
        string slotId = "Slot_01";
        savePath = Path.Combine(Application.persistentDataPath, $"Auto_{slotId}.es3");
        continueGameButton.gameObject.SetActive(File.Exists(savePath));

        StartCoroutine(UI_TransitionController.instance.Fade(1, 0, 1f));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Start()
    {
        newGameButton.onClick.AddListener(OnClickNewGame);

        noNewGameButton.onClick.AddListener(() => newGameConfirmationPanel.SetActive(false));
        yesNewGameButton.onClick.AddListener(OnClickYesNewGame);

        continueGameButton.onClick.AddListener(OnClickContinueGame);

        closeGameButton.onClick.AddListener(OnClickCloseGame);
        yesCloseGameButton.onClick.AddListener(OnClickYesCloseGame);
        noCloseGameButton.onClick.AddListener(OnClickNoCloseGame);

        settingsButton.onClick.AddListener(OnOpenSettings);
        returnButton.onClick.AddListener(OnCloseSettings);
        audioButton.onClick.AddListener(OnClickAudio);
        displayAndGraphicsButton.onClick.AddListener(OnClickDisplayAndGraphics);
        controlButton.onClick.AddListener(OnClickControl);
    }

    private void OnClickNewGame()
    {
        newGameConfirmationPanel.SetActive(true);
    }

    private void OnClickYesNewGame() => StartCoroutine(NewGame());

    public void OnClickNoNewGame()
    {
        newGameConfirmationPanel.SetActive(false);
    }
    IEnumerator NewGame()
    {
        GameManager.instance.DeleteAll();
        StartCoroutine(UI_TransitionController.instance.Fade(0, 1, 1f));
        yield return new WaitForSeconds(1f);
        Loader.Load(3);
    }

    private void OnClickContinueGame() => StartCoroutine(ContinueGame());

    IEnumerator ContinueGame()
    {
        StartCoroutine(UI_TransitionController.instance.Fade(0, 1, 1f));
        yield return new WaitForSeconds(1f);
        Loader.Load(4);
    }

    private void OnClickCloseGame()
    {
        closeGameConfirmationPanel.SetActive(true);
    }

    private void OnClickYesCloseGame() => StartCoroutine(CloseGame());

    private void OnClickNoCloseGame()
    {
        closeGameConfirmationPanel.SetActive(false);
    }

    private void OnOpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    private void OnCloseSettings()
    {
        settingsPanel.SetActive(false);
    }
    public void OnClickAudio()
    {
        audioPanel.SetActive(true);
        displayAndGraphicsPanel.SetActive(false);
        controlPanel.SetActive(false);
    }
    public void OnClickDisplayAndGraphics()
    {
        audioPanel.SetActive(false);
        displayAndGraphicsPanel.SetActive(true);
        controlPanel.SetActive(false);

    }
    public void OnClickControl()
    {
        audioPanel.SetActive(false);
        displayAndGraphicsPanel.SetActive(false);
        controlPanel.SetActive(true);
    }

    IEnumerator CloseGame()
    {
        StartCoroutine(UI_TransitionController.instance.Fade(0, 1, 1f));
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

}
