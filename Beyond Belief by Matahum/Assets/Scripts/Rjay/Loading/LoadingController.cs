using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // for Image
using TMPro;

public class LoadingController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fillImage;   // Image (Type = Filled, Horizontal)
    [SerializeField] private TextMeshProUGUI percentText; // optional (or TMP_Text if you use TMP)

    [Header("Behavior")]
    [SerializeField, Min(0f)] private float minShowTime = 0.35f; // optional minimum display

    private void Start()
    {
        if (fillImage) fillImage.fillAmount = 0f;
        StartCoroutine(LoadNext());
    }

    private IEnumerator LoadNext()
    {
        yield return null; // give UI a frame to render

        var op = SceneManager.LoadSceneAsync(Loader.NextSceneBuildIndex, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        float startTime = Time.time;

        // Unity progress goes 0..0.9 until we allow activation
        while (op.progress < 0.9f)
        {
            float t = Mathf.Clamp01(op.progress / 0.9f);
            if (fillImage) fillImage.fillAmount = t;
            if (percentText) percentText.text = Mathf.RoundToInt(t * 100f) + "%";
            yield return null;
        }

        // Smooth to 100% visually
        if (fillImage) fillImage.fillAmount = 1f;
        if (percentText) percentText.text = "100%";

        // Optional: ensure loading screen is shown at least a short time
        float elapsed = Time.time - startTime;
        if (elapsed < minShowTime) yield return new WaitForSeconds(minShowTime - elapsed);

        // Now switch scenes
        op.allowSceneActivation = true;
    }
}
