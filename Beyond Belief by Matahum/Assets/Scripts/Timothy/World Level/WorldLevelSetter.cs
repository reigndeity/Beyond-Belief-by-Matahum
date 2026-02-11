using UnityEngine;

public class WorldLevelSetter : MonoBehaviour
{
    public static WorldLevelSetter Instance;
    public int worldLevel = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // destroy the whole GameObject, not just the component
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        worldLevel = PlayerPrefs.GetInt("WorldLevel", 1);
    }

    public void SetWorldLevel(int level)
    {
        worldLevel = level;
        PlayerPrefs.SetInt("WorldLevel", worldLevel);
        PlayerPrefs.Save();
    }
}
