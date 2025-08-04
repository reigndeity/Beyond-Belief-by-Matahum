using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BB_ArchivesCategoryButtonPopUp : MonoBehaviour
{
    [Header("Buttons to Animate")]
    public List<RectTransform> buttons;

    [Header("Animation Settings")]
    public float moveDistance = 20f;
    public float returnDistance = 0f;
    public float preMoveDistance = 5f;
    public float preMoveDuration = 0.1f;
    public float moveDuration = 0.2f;

    private Coroutine currentCoroutine;
    private RectTransform currentlySelected;

    public void HighlightButton(RectTransform selectedButton)
    {
        if (selectedButton == currentlySelected)
            return; // Don't animate if re-selecting the same button

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(AnimateHighlightTransition(currentlySelected, selectedButton));
        currentlySelected = selectedButton;
    }

    private IEnumerator AnimateHighlightTransition(RectTransform oldButton, RectTransform newButton)
    {
        if (oldButton != null)
            StartCoroutine(BobThenMove(oldButton, -preMoveDistance, returnDistance)); // unselect old

        if (newButton != null)
            StartCoroutine(BobThenMove(newButton, preMoveDistance, -moveDistance)); // highlight new

        yield return null;
    }

    private IEnumerator BobThenMove(RectTransform button, float bobOffset, float finalOffset)
    {
        Vector2 startPos = button.anchoredPosition;
        Vector2 bobTarget = new Vector2(startPos.x + bobOffset, startPos.y);

        // Step 1: Bob
        float elapsed = 0f;
        while (elapsed < preMoveDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / preMoveDuration);
            button.anchoredPosition = Vector2.Lerp(startPos, bobTarget, t);
            yield return null;
        }

        // Step 2: Move to final position
        Vector2 moveStart = button.anchoredPosition;
        Vector2 finalTarget = new Vector2(finalOffset, startPos.y);

        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            button.anchoredPosition = Vector2.Lerp(moveStart, finalTarget, t);
            yield return null;
        }

        button.anchoredPosition = finalTarget;
    }
}
