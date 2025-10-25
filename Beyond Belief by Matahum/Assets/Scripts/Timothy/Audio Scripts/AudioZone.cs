using System.Collections;
using UnityEngine;

public class AudioZone : MonoBehaviour
{
    public AudioClip backgroundMusicClip;
    public AudioClip ambienceMusicClip;

    public float audioDistance = 100;

    private Transform player;
    private bool isPlaying = false;

    private void Start()
    {
        player = FindFirstObjectByType<Player>().transform;
    }

    private void Update()
    {
        if (player == null) return;   

        if (Vector3.Distance(player.position, transform.position) < audioDistance)
        {
            if (!isPlaying && !AudioManager.instance.lockMusic)
            {
                isPlaying = true;

                StartCoroutine(AudioManager.instance.FadeMusic(backgroundMusicClip));
                StartCoroutine(AudioManager.instance.FadeAmbience(ambienceMusicClip));
            }            
        }
        else
        {
            isPlaying = false;
        }
            
    }

    // 🎨 Draws the audio zone in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.3f); // light blue, semi-transparent
        Gizmos.DrawSphere(transform.position, audioDistance);

        Gizmos.color = new Color(0.2f, 0.6f, 1f, 1f); // blue outline
        Gizmos.DrawWireSphere(transform.position, audioDistance);
    }
}
