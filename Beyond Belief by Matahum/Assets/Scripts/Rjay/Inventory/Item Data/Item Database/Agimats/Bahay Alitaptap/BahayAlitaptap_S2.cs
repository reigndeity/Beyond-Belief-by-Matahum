using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Agimat/Abilities/BahayAlitaptap_S2")]
public class BahayAlitaptap_S2 : R_AgimatAbility
{
    public override string GetDescription(R_ItemRarity rarity)
    {
        return $"Undiscovered Creatures, Locations, Wildlife, and Plants have visual indicators.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity)
    {
        BB_ArchiveTracker[] allTrackers = Resources.FindObjectsOfTypeAll<BB_ArchiveTracker>();
        List<BB_ArchiveTracker> sceneTrackers = new List<BB_ArchiveTracker>();

        foreach (var tracker in allTrackers)
        {
            if (tracker.gameObject.scene.isLoaded) // ignore prefabs/assets
            {
                sceneTrackers.Add(tracker);
            }
        }

        foreach (var tracker in sceneTrackers)
        {
            CoroutineRunner.Instance.RunCoroutine(DiscoverablePopUp(tracker));
        }
    }

    IEnumerator DiscoverablePopUp(BB_ArchiveTracker tracker)
    {
        // If already discovered, make sure it's hidden and exit
        if (tracker.archiveData.isDiscovered)
        {
            tracker.canvasGroup.alpha = 0f;
            yield break;
        }

        // Instantly show
        tracker.canvasGroup.alpha = 1f;

        // Wait visible duration
        yield return new WaitForSeconds(5f);

        // Fade out over 3 seconds
        float fadeDuration = 3f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            if (tracker.archiveData.isDiscovered) // check mid-fade
            {
                tracker.canvasGroup.alpha = 0f;
                yield break;
            }

            elapsed += Time.deltaTime;
            tracker.canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        tracker.canvasGroup.alpha = 0f; // ensure fully hidden
    }

}
