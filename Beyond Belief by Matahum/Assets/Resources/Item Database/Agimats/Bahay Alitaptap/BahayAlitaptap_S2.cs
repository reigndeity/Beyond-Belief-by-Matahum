using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/BahayAlitaptap/BahayAlitaptap_S2")]
public class BahayAlitaptap_S2 : R_AgimatAbility
{
    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        return $"Undiscovered Creatures, Locations, Wildlife, and Plants have visual indicators.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
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

    private IEnumerator DiscoverablePopUp(BB_ArchiveTracker tracker)
    {
        if (tracker.archiveData.isDiscovered)
        {
            tracker.canvasGroup.alpha = 0f;
            yield break;
        }

        tracker.canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(5f);

        float fadeDuration = 3f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            if (tracker.archiveData.isDiscovered)
            {
                tracker.canvasGroup.alpha = 0f;
                yield break;
            }

            elapsed += Time.deltaTime;
            tracker.canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        tracker.canvasGroup.alpha = 0f;
    }

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        // This ability doesn’t use rolls
        return 0f;
    }
}
