using UnityEngine;
using System.Collections.Generic;
using System;

public class BB_ArchiveManager : MonoBehaviour
{
    public static BB_ArchiveManager instance;

    [Header("All available Quests")]
    public List<BB_ArchiveSO> allArchives = new List<BB_ArchiveSO>();

    [Header("Archive Lists")]
    public List<BB_ArchiveSO> creatureList = new List<BB_ArchiveSO>();
    public List<BB_ArchiveSO> locationList = new List<BB_ArchiveSO>();
    public List<BB_ArchiveSO> wildlifeList = new List<BB_ArchiveSO>();
    public List<BB_ArchiveSO> plantList = new List<BB_ArchiveSO>();

    public event Action<BB_ArchiveSO> OnArchiveUpdate;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            allArchives = new List<BB_ArchiveSO>(Resources.LoadAll<BB_ArchiveSO>("Archives"));
            allArchives.Sort((a, b) => a.name.CompareTo(b.name));
        }
        else Destroy(gameObject);
    }

    /*private void Start()
    {
        foreach (var archive in allArchives)
        {
            archive.isDiscovered = PlayerPrefs.GetInt($"{archive.archiveName}_Discovered", 0) == 1;
            archive.isViewed = PlayerPrefs.GetInt($"{archive.archiveName}_Viewed", 0) == 1;
        }
    }*/
    public void SettingUpArchives()
    {
        foreach (var obj in allArchives)
        {
            switch (obj.archiveType)
            {
                case ArchiveType.creature:
                    creatureList.Add(obj);
                    break;

                case ArchiveType.location:
                    locationList.Add(obj);
                    break;

                case ArchiveType.wildlife: 
                    wildlifeList.Add(obj);
                    break;

                case ArchiveType.plant: 
                    plantList.Add(obj);
                    break;
            }
        }
    }

    public void UpdateArchive(BB_ArchiveSO archiveData)
    {
        foreach (var obj in creatureList)       
            CheckArchive(obj, archiveData);

        foreach (var obj in locationList)
            CheckArchive(obj, archiveData);

        foreach (var obj in wildlifeList)
            CheckArchive(obj, archiveData);

        foreach (var obj in plantList)
            CheckArchive(obj, archiveData);
    }

    public void CheckArchive(BB_ArchiveSO archiveObj, BB_ArchiveSO archiveData)
    {
        bool isDiscovered = PlayerPrefs.GetInt($"{archiveObj.archiveName}_Discovered", 0) == 1;
        if (archiveObj == archiveData && !isDiscovered)
        {
            //archiveObj.isDiscovered = true;
            PlayerPrefs.SetInt($"{archiveObj.archiveName}_Discovered", 1);
            PlayerPrefs.Save();
            OnArchiveUpdate?.Invoke(archiveObj);
            Debug.Log(archiveObj.archiveName + " has been discovered (visible and unobstructed)");
        }
    }

    public void ResetArchiveDiscovery()
    {
        foreach (var archive in allArchives)
        {
            archive.isDiscovered = false;
            archive.isViewed = false;
        }
    }
}
