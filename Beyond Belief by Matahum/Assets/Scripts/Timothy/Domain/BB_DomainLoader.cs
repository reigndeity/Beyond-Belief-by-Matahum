using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BB_DomainLoader : MonoBehaviour
{
    private async void Start()
    {
        await SaveManager.Instance.LoadSystemsAsync("slot_01", false, "Equipment.Main", "Inventory.Main", "Player.Stats");
    }
}
