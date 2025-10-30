using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Agimat/Abilities/BahayAlitaptap/BahayAlitaptap_S2")]
public class BahayAlitaptap_S2 : R_AgimatAbility
{
    public GameObject sparkleVFX;
    private readonly List<GameObject> sparkleList = new List<GameObject>();

    public override string GetDescription(R_ItemRarity rarity, R_ItemData itemData)
    {
        return $"Undiscovered Creatures, Locations, Wildlife, and Plants have visual indicators.";
    }

    public override void Activate(GameObject user, R_ItemRarity rarity, R_ItemData itemData)
    {
        // Clean up old sparkles before adding new ones
        foreach (var sparkle in sparkleList)
        {
            if (sparkle != null)
                Destroy(sparkle);
        }
        sparkleList.Clear();

        BB_ArchiveTracker[] allTrackers = Resources.FindObjectsOfTypeAll<BB_ArchiveTracker>();
        foreach (var tracker in allTrackers)
        {
            if (tracker.gameObject.scene.isLoaded) // ignore prefabs/assets
            {
                CoroutineRunner.Instance.RunCoroutine(DiscoverablePopUp(tracker));
            }
        }
    }

    public override void Deactivate(GameObject user)
    {
        // Clean up old sparkles before adding new ones
        foreach (var sparkle in sparkleList)
        {
            if (sparkle != null)
                Destroy(sparkle);
        }
        sparkleList.Clear();
    }

    private IEnumerator DiscoverablePopUp(BB_ArchiveTracker tracker)
    {
        Vector3 offset = new Vector3(0, 1, 0);
        GameObject sparkleObj = Instantiate(sparkleVFX, tracker.transform.position + offset, Quaternion.identity, tracker.transform);
        sparkleList.Add(sparkleObj);

        bool isDiscovered = PlayerPrefs.GetInt($"{tracker.archiveData.archiveName}_Discovered", 0) == 1;

        if (isDiscovered)
        {
            Destroy(sparkleObj);
            sparkleList.Remove(sparkleObj);
            yield break;
        }

        yield return new WaitForSeconds(5f);

        float fadeDuration = 3f;
        float elapsed = 0f;
        CanvasGroup cg = tracker.canvasGroup;

        while (elapsed < fadeDuration)
        {
            
            if (isDiscovered)
            {
                if (cg != null) cg.alpha = 0f;
                Destroy(sparkleObj);
                sparkleList.Remove(sparkleObj);
                yield break;
            }

            elapsed += Time.deltaTime;
            if (cg != null)
                cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            yield return null;
        }

        Destroy(sparkleObj);
        sparkleList.Remove(sparkleObj);
        if (cg != null) cg.alpha = 0f;
    }

    public override float GetRandomDamagePercent(R_ItemRarity rarity)
    {
        return 0f; // This ability doesn’t use rolls
    }
}
