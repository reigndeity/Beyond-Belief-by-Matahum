using UnityEngine;
using TMPro;

public class PamanaDescription : MonoBehaviour
{
    [Header("Pamana Name")]
    public TextMeshProUGUI pamanaName;

    [Header("Pamana Main and Sub stats")]
    public TextMeshProUGUI mainStatName;
    public TextMeshProUGUI mainStatValue;
    public TextMeshProUGUI subStatName;
    public TextMeshProUGUI subStatValue;

    [Header("Pamana Set Pieces")]
    public TextMeshProUGUI setPieceName;
    public TextMeshProUGUI firstSetPieceValue;
    public TextMeshProUGUI secondSetPieceValue;
}
