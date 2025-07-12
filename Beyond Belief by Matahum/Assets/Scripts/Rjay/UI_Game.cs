using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
    
    [SerializeField] Button inventoryButton; // Overall Inventory (No Equipment)
    [SerializeField] Button journalButton; // Keeps track of new findings(?)

    [Header("Character Details Properties")]
    [SerializeField] Button characterButton; // Handles Dialogue, Pamana Equipment, Agimat Equipment
    [SerializeField] GameObject characterPanel;

    [Header("Full Screen Map Properties")]
    [SerializeField] GameObject teleportPanel;
    [SerializeField] Button closeTeleportPanelButton;
    [SerializeField] Button teleportButton;
    public static event Action OnCloseTeleportPanel;

    void Start()
    {
        closeTeleportPanelButton.onClick.AddListener(OnClickCloseTeleportPanel);
        teleportButton.onClick.AddListener(OnClickTeleport);
    }

    public void OnClickCloseTeleportPanel()
    {
        teleportPanel.SetActive(false);
        MapTeleportManager.instance.HideSelection();
    }
    public void OnClickTeleport()
    {
        MapTeleportManager.instance.TeleportPlayerToSelected();
    }
}
