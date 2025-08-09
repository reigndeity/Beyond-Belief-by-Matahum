using UnityEngine;
using UnityEngine.UI;

public class BB_DomainEntranceInteractable : Interactable
{
    public GameObject confirmationPanel;


    public override void OnInteract()
    {
        confirmationPanel.SetActive(true);
    }
}
