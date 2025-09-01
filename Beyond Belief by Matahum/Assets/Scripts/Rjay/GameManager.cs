using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private SceneAutoSaveController m_sceneAutoSaveController;
    void Awake()
    {
        m_sceneAutoSaveController = FindFirstObjectByType<SceneAutoSaveController>();
    }
    void Start()
    {
        //StartCoroutine(LoadPlayer());
        StartCoroutine(TemporaryLoadPlayer());
    }
    IEnumerator LoadPlayer()
    {
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        yield return new WaitForSeconds(1f);
        m_sceneAutoSaveController.LoadAll();
        PlayerCamera.Instance.AdjustCamera();
        yield return new WaitForSeconds(1f);
        StartCoroutine(UI_TransitionController.instance.Fade(1f, 0f, 0.5f));
    }
    IEnumerator TemporaryLoadPlayer()
    {
        StartCoroutine(UI_TransitionController.instance.Fade(0f, 1f, 0.5f));
        yield return new WaitForSeconds(1f);
        PlayerCamera.Instance.AdjustCamera();
        yield return new WaitForSeconds(1f);
        StartCoroutine(UI_TransitionController.instance.Fade(1f, 0f, 0.5f));  
    }

}
