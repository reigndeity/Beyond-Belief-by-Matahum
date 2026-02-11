using UnityEngine;

public class cutsceneLoader : MonoBehaviour
{
    public void LoadOpenWorldScene()
    {
        Loader.Load(4);
    }
    public void LoadNunoBossFightScene()
    {
        Loader.Load(6);
    }
}
