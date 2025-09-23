using UnityEngine;

public class TimelineUtilities : MonoBehaviour
{
    public void LoadNextScene(int index)
    {
        Loader.Load(index);
    }
}
