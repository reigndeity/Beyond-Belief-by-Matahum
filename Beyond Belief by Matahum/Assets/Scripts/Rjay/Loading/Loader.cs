using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    // Set this before we jump to the Loading scene
    public static int NextSceneBuildIndex;

    // Call this from your menu: Loader.Load(sceneIndex);
    public static void Load(int targetBuildIndex)
    {
        NextSceneBuildIndex = targetBuildIndex;
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single); // open the loading scene
    }
}
