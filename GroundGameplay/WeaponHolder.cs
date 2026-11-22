using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public IWeapon currentWeapon;
    public MonoBehaviour[] weapons;

    void Start()
    {
        EquipWeapon(0);
    }

    public void EquipWeapon(int index)
    {
        if (index >= weapons.Length) return;

        foreach (MonoBehaviour weapon in weapons)
            weapon.gameObject.SetActive(false);

        weapons[index].gameObject.SetActive(true);
        currentWeapon = weapons[index].GetComponent<IWeapon>();
    }
}