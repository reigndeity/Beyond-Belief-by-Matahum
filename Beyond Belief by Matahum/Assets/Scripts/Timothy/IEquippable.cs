using UnityEngine;

public interface IEquippable
{
    void Equip();
    void Equip(ItemSlot itemSlot);
    void Unequip();

    bool isEquipped { get; set; }
}
