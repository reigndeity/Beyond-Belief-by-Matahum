using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public bool isTutorialDone;

    [Header("Tutorial Components")]
    public UI_CanvasGroup characterDetailsButton;
    public UI_CanvasGroup inventoryButton;
    public UI_CanvasGroup archiveButton;
    public UI_CanvasGroup questButton;
    public UI_CanvasGroup minimap;
    public UI_CanvasGroup normalSkill;
    public UI_CanvasGroup ultimatSkill;
    public UI_CanvasGroup agimatOne;
    public UI_CanvasGroup agimatTwo;
    public UI_CanvasGroup health;

    [Header("Tupas House")]
    public GameObject temporaryCollider;
    public GameObject tupasHouseStairs;
    public DoorInteractable tupasHouseDoor;
    public GameObject cutsceneTriggerOne;

    void Start()
    {
        if (!isTutorialDone)
        {
            StartTutorial();
        } 
    }
    void Update()
    {
        if (!isTutorialDone)
        {
            if (tupasHouseDoor.isOpen)
            {
                cutsceneTriggerOne.SetActive(true);
            }
            else
            {
                cutsceneTriggerOne.SetActive(false);
            }
        }
        else
        {
            return;
        }
    }

    void StartTutorial()
    {
        PlayerCamera.Instance.HardLockCamera();
        BB_QuestManager.Instance.AcceptQuestByID("A0_Q0_InitialTalk");
        characterDetailsButton.FadeOut(0);
        inventoryButton.FadeOut(0);
        archiveButton.FadeOut(0);
        questButton.FadeOut(0);
        minimap.FadeOut(0);
        normalSkill.FadeOut(0);
        ultimatSkill.FadeOut(0);
        agimatOne.FadeOut(0);
        agimatTwo.FadeOut(0);   
        health.FadeOut(0);
        tupasHouseStairs.SetActive(false);
        temporaryCollider.SetActive(true);
        tupasHouseDoor.interactCooldown = 9999f;
    }
}
