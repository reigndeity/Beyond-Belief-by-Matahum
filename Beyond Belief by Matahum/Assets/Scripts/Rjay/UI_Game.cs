using UnityEngine;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
    
    [SerializeField] Button inventoryButton; // Overall Inventory (No Equipment)
    [SerializeField] Button journalButton; // Keeps track of new findings(?)

    [Header("Character Details Properties")]
    [SerializeField] Button characterButton; // Handles Dialogue, Pamana Equipment, Agimat Equipment
    [SerializeField] GameObject characterPanel;
}
