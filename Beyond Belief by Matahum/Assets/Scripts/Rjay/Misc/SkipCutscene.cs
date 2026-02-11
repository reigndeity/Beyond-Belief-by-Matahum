using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkipCutscene : MonoBehaviour
{
    public int cutsceneToChange;

    [Header("UI")]
    public Image chargeImage;   // Assign your filled Image
    [Header("Fade")]
    public UI_CanvasGroup fadeCanvas;

    [Header("Charge Settings")]
    public float chargeTime = 3f;       // Time to fully charge
    public float decreaseSpeed = 0.5f;  // Speed when bar decreases

    private float currentCharge = 0f;
    private bool skipTriggered = false; // prevent multiple triggers

    void Update()
    {
        if (skipTriggered)
            return; // do nothing once skip is triggered

        if (Input.GetKey(KeyCode.Space))
        {
            // Fill up over chargeTime
            currentCharge += Time.deltaTime / chargeTime;
        }
        else
        {
            // Slowly decrease only if not full yet
            currentCharge -= Time.deltaTime * decreaseSpeed;
        }

        // Clamp between 0 and 1
        currentCharge = Mathf.Clamp01(currentCharge);

        // Update UI image fill
        if (chargeImage != null)
            chargeImage.fillAmount = currentCharge;

        // Check if fully charged
        if (currentCharge >= 1f && !skipTriggered)
        {
            skipTriggered = true;
            StartCoroutine(SkippingScene());
        }
    }

    IEnumerator SkippingScene()
    {
        fadeCanvas.FadeIn(0.5f);
        yield return new WaitForSeconds(2f);
        Loader.Load(cutsceneToChange);
    }
}
