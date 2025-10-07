using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class HaligiInteractable : Interactable
{
    [SerializeField] private PlayableDirector haligiDirector;
    [SerializeField] private TimelineAsset repairHaligi;
    public string questMissionID;
    public GameObject fragment;

    public UnityEvent OnInteraction;

        public override void OnInteract()
    {
        if (useInteractCooldown && IsOnCooldown()) return;

        base.OnInteract();
        haligiDirector.Play(repairHaligi);
        BB_QuestManager.Instance.UpdateMissionProgressOnce(questMissionID);
        fragment.SetActive(true);
        OnInteraction?.Invoke();
    }
}
