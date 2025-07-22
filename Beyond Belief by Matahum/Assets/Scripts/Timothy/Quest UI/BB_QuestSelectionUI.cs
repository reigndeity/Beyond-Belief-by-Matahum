using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BB_QuestSelectionUI : MonoBehaviour
{
    public GameObject questGroupTemplate;
    public GameObject questButtonTemplate;

    public Transform mainQuestSelectionScrollContent;
    public Transform sideQuestSelectionScrollContent;

    private Dictionary<string, GameObject> activeGroups = new();

    private Dictionary<string, GameObject> questButtonLookup = new Dictionary<string, GameObject>();
    private List<BB_Quest> allQuests = new List<BB_Quest>(); // optional: if you want logic-based quest tracking


    public void PopulateQuestList(List<BB_Quest> quests)
    {
        // Clear UI first
        foreach (Transform child in mainQuestSelectionScrollContent) Destroy(child.gameObject);
        foreach (Transform child in sideQuestSelectionScrollContent) Destroy(child.gameObject);
        activeGroups.Clear();

        Dictionary<string, List<BB_Quest>> actQuestMap_Main = new Dictionary<string, List<BB_Quest>>();
        Dictionary<string, List<BB_Quest>> actQuestMap_Side = new Dictionary<string, List<BB_Quest>>();

        // Group quests into act-based maps
        foreach (var quest in quests)
        {
            var map = (quest.questType == BB_QuestType.Main) ? actQuestMap_Main : actQuestMap_Side;
            if (!map.ContainsKey(quest.actNumber))
                map[quest.actNumber] = new List<BB_Quest>();
            map[quest.actNumber].Add(quest);
        }

        // Render UI for each type
        RenderQuestGroupUI(actQuestMap_Main, mainQuestSelectionScrollContent);
        RenderQuestGroupUI(actQuestMap_Side, sideQuestSelectionScrollContent);
    }

    private void RenderQuestGroupUI(Dictionary<string, List<BB_Quest>> actMap, Transform parent)
    {
        foreach (var kvp in actMap)
        {
            string actName = kvp.Key;
            List<BB_Quest> questsInAct = kvp.Value;

            GameObject actGroup = Instantiate(questGroupTemplate, parent);
            actGroup.transform.Find("Act Title").GetComponentInChildren<TMPro.TextMeshProUGUI>().text = actName;

            List<GameObject> questButtons = new List<GameObject>();

            foreach (var quest in questsInAct)
            {
                GameObject questBtn = Instantiate(questButtonTemplate, actGroup.transform);
                var questText = questBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                questText.text = quest.questTitle;
                questBtn.SetActive(false);

                // 🔑 Add to lookup
                if (!questButtonLookup.ContainsKey(quest.questID))
                    questButtonLookup[quest.questID] = questBtn;

                questButtons.Add(questBtn);
            }

            bool expanded = false;
            Button toggleBtn = actGroup.transform.Find("Act Title").GetComponent<Button>();
            toggleBtn.onClick.AddListener(() =>
            {
                expanded = !expanded;
                foreach (var btn in questButtons)
                    btn.SetActive(expanded);
            });

            activeGroups[actName] = actGroup;
        }
    }

    public void MarkQuestAsCompleted(string questID)
    {
        if (questButtonLookup.TryGetValue(questID, out GameObject questBtn))
        {
            // ✅ Example: visually show completed quest
            var text = questBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            text.text = "[Completed] " + text.text;

            // Optional: change color
            text.color = Color.green;

            // Optional: disable interaction
            var btn = questBtn.GetComponent<Button>();
            if (btn != null)
                btn.interactable = false;

            Debug.Log($"Quest {questID} marked as completed.");
        }
        else
        {
            Debug.LogWarning($"Quest ID {questID} not found.");
        }
    }
}
