using UnityEngine;

public enum ArchiveType
{
    creature,
    location,
    wildlife,
    plant
}

[CreateAssetMenu(menuName = "Beyond Belief/Archives")]
public class BB_ArchiveSO : ScriptableObject
{
    public string archiveName;
    public Sprite archiveImage;
    [TextArea(1,3)]
    public string archiveDescription;
    public ArchiveType archiveType;

    public bool isDiscovered;
}
