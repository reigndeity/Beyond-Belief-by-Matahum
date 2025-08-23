using UnityEngine;
using UnityEngine.Playables;

public class npcTimelineController : MonoBehaviour
{
    public PlayableDirector myPlayableDirector;
    public Transform npcTransform;

    private bool wasPlaying = false;
    private bool finalPositionSet = false;

    void Update()
    {
        // Check if the timeline was playing and has now been paused
        if (myPlayableDirector.state == PlayState.Paused && wasPlaying && !finalPositionSet)
        {
            // Capture the NPC's position at the moment the timeline was paused
            Vector3 pausedPosition = npcTransform.position;

            // Set the final position to the captured paused position
            npcTransform.position = pausedPosition;

            // Set a flag so this logic only runs once when paused
            finalPositionSet = true;

            Debug.Log("Timeline was paused. NPC's final position has been set to: " + pausedPosition);
        }

        // Reset the flag if the timeline starts playing again
        if (myPlayableDirector.state == PlayState.Playing)
        {
            wasPlaying = true;
            finalPositionSet = false;
        }
        else
        {
            wasPlaying = false;
        }
    }
}