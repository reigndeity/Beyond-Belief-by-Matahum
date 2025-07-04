using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PamanaSetPieces : MonoBehaviour
{
    public List<Pamana> EquippedPamana = new List<Pamana>(); // List of currently equipped Pamana
    public Dictionary<string, int> setCount = new Dictionary<string, int>();
    //private PlayerStats playerStats;
    private Temp_PlayerStats playerStats;
    public PamanaInventory pamanaEquipmentHolder;

    private void Start()
    {
        playerStats = FindFirstObjectByType<Temp_PlayerStats>();

        if (pamanaEquipmentHolder.diwataSlot.GetComponentInChildren<PamanaUI>() != null)
        {
            EquipPamana(pamanaEquipmentHolder.diwataSlot.GetComponentInChildren<Pamana>());
            Debug.Log("Initial Diwata Set Piece");
        }
        if (pamanaEquipmentHolder.lihimSlot.GetComponentInChildren<PamanaUI>() != null)
        {
            EquipPamana(pamanaEquipmentHolder.lihimSlot.GetComponentInChildren<Pamana>());
            Debug.Log("Initial Lihim Set Piece");

        }
        if (pamanaEquipmentHolder.SalamangkeroSlot.GetComponentInChildren<PamanaUI>() != null)
        {
            EquipPamana(pamanaEquipmentHolder.SalamangkeroSlot.GetComponentInChildren<Pamana>());
            Debug.Log("Initial Salamangkero Set Piece");

        }
    }
    public void UpdateSetBonus()
    {
        foreach (var set in setCount)
        {
            if (set.Value == 2)
                ApplyBonus(set.Key, 1); // Bonus for 2 matching pieces

            else if (set.Value == 3)
                ApplyBonus(set.Key, 2); // Bonus for 3 matching pieces
        }

    }

    public void EquipPamana(Pamana pamana)
    {
        if (EquippedPamana.Count < 3)
        {
            EquippedPamana.Add(pamana);

            if (setCount.ContainsKey(pamana.setPiece.setPieceName.ToString()))
            {
                setCount[pamana.setPiece.setPieceName.ToString()]++;
            }
            else setCount[pamana.setPiece.setPieceName.ToString()] = 1;

            UpdateSetBonus(); // Recalculate bonuses
        }
        else
        {
            Debug.LogWarning("Maximum of 3 Pamana can be equipped!");
        }
    }

    public void UnequipPamana(Pamana pamana)
    {
        if (EquippedPamana.Contains(pamana))
        {
            EquippedPamana.Remove(pamana);

            if (setCount.ContainsKey(pamana.setPiece.setPieceName.ToString()))
            {
                if (setCount[pamana.setPiece.setPieceName.ToString()] > 1)
                {
                    RemoveBonus(pamana.setPiece.setPieceName.ToString(), setCount[pamana.setPiece.setPieceName.ToString()]);
                }

                setCount[pamana.setPiece.setPieceName.ToString()]--;

                if (setCount[pamana.setPiece.setPieceName.ToString()] <= 0)
                    setCount.Remove(pamana.setPiece.setPieceName.ToString());
            }

            //UpdateSetBonus(); // Recalculate bonuses
        }
    }



    private void ApplyBonus(string setType, int level)
    {
        if (setCount.ContainsKey(setType))
        {
            switch (setType)
            {
                case "Katiwala_ng_Araw":
                    if (level <= 1) playerStats.p_hpPrcnt += 10f;
                    if (level == 2) playerStats.p_atkPrcnt += 15f;
                    break;
                case "Hinagpis_ng_Digmaan":
                    if (level == 1) playerStats.p_critRate += 10f;
                    if (level == 2) playerStats.p_critDmg += 15f;
                    break;
                case "Katiwala_ng_Buwan":
                    if (level == 1) playerStats.p_hpPrcnt += 10f;
                    if (level == 2) playerStats.p_defPrcnt += 15f;
                    break;
                case "Malabulkang_Maniniil":
                    if (level == 1) playerStats.p_flatAtk += 50f;
                    if (level == 2) playerStats.p_atkPrcnt += 15f;
                    break;
                case "Tagapangasiwa_ng_Layog":
                    if (level == 1) playerStats.p_flatHP += 100f;
                    if (level == 2) playerStats.p_hpPrcnt += 15f;
                    break;
                case "Soberano_ng_Unos":
                    if (level == 1) playerStats.p_atkPrcnt += 10f;
                    if (level == 2) playerStats.p_critRate += 15f;
                    break;
                case "Yakap_ng_Ilog":
                    if (level == 1) playerStats.p_defPrcnt += 10f;
                    if (level == 2) playerStats.p_hpPrcnt += 15f;
                    break;
                case "Bulong_sa_Kagubatan":
                    if (level == 1) playerStats.p_flatDef += 50f;
                    if (level == 2) playerStats.p_defPrcnt += 15f;
                    break;
                case "Propeta_ng_Kamatayan":
                    if (level == 1) playerStats.p_critDmg += 10f;
                    if (level == 2) playerStats.p_atkPrcnt += 15f;
                    break;
                default:
                    break;
            }
        }
    }
    private void RemoveBonus(string setType, int level)
    {
        switch (setType)
        {
            case "Katiwala_ng_Araw":
                if (level <= 1) playerStats.p_hpPrcnt -= 10f;
                if (level == 2) playerStats.p_atkPrcnt -= 15f;
                break;
            case "Hinagpis_ng_Digmaan":
                if (level == 1) playerStats.p_critRate -= 10f;
                if (level == 2) playerStats.p_critDmg -= 15f;
                break;
            case "Katiwala_ng_Buwan":
                if (level == 1) playerStats.p_hpPrcnt -= 10f;
                if (level == 2) playerStats.p_defPrcnt -= 15f;
                break;
            case "Malabulkang_Maniniil":
                if (level == 1) playerStats.p_flatAtk -= 50f;
                if (level == 2) playerStats.p_atkPrcnt -= 15f;
                break;
            case "Tagapangasiwa_ng_Layog":
                if (level == 1) playerStats.p_flatHP -= 100f;
                if (level == 2) playerStats.p_hpPrcnt -= 15f;
                break;
            case "Soberano_ng_Unos":
                if (level == 1) playerStats.p_atkPrcnt -= 10f;
                if (level == 2) playerStats.p_critRate -= 15f;
                break;
            case "Yakap_ng_Ilog":
                if (level == 1) playerStats.p_defPrcnt -= 10f;
                if (level == 2) playerStats.p_hpPrcnt -= 15f;
                break;
            case "Bulong_sa_Kagubatan":
                if (level == 1) playerStats.p_flatDef -= 10f;
                if (level == 2) playerStats.p_defPrcnt -= 50f;
                break;
            case "Propeta_ng_Kamatayan":
                if (level == 1) playerStats.p_critDmg -= 10f;
                if (level == 2) playerStats.p_atkPrcnt -= 15f;
                break;
            default:
                break;
        }
    }

}
