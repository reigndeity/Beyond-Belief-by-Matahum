using UnityEngine;

public class BB_ArchiveHUD : MonoBehaviour
{
    [Header("Discovery Alert")]
    public BB_ArchiveHUDAlert alertPrefab;
    public Transform hudAlertHolder;

    private void Start()
    {
        BB_ArchiveManager.instance.OnArchiveUpdate += SpawnAlert;
    }

    void SpawnAlert(BB_ArchiveSO archiveObj)
    {
        BB_ArchiveHUDAlert alertObj = Instantiate(alertPrefab, hudAlertHolder);
        alertObj.hudAlertText.text = $"{archiveObj.archiveName} discovered!";
        Destroy(alertObj.gameObject, 5f);
    }
}
