using System.Threading.Tasks;
using UnityEngine;

public class Nuno_GameStart : MonoBehaviour
{
    [SerializeField] UI_CanvasGroup nunoCanvasGroup;


    void Start()
    {
        CustomLoadPlayer();
    }

    private void CustomLoadPlayer()
    {
        nunoCanvasGroup.FadeIn(1f);
        BattleStart();
    }

    public void BattleStart()
    {
        Nuno_AttackManager.Instance.isBattleStart = true;
    }

    public void DisableUI()
    {
        TutorialManager.instance.HideMinimap();
        TutorialManager.instance.HideCharacterDetails();
        TutorialManager.instance.HideQuestJournal();
        TutorialManager.instance.HideInventory();
        TutorialManager.instance.DisableFullscreenMap();
        TutorialManager.instance.HideArchives();
    }
}
