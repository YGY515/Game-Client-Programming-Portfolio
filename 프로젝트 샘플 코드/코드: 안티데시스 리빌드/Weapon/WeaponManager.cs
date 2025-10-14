using UnityEngine;


public class WeaponManager : MonoBehaviour
{
    public WeaponData weaponData;
    public GameObject[] weaponObjects;
    private IWeapon currentWeapon;

    
    void Start()
    {
        currentWeapon = weaponObjects[weaponData.weapon_index].GetComponent<IWeapon>();

        if (currentWeapon == null)
            Debug.LogError($"{weaponObjects[weaponData.weapon_index].name} ������Ʈ�� IWeapon ������Ʈ�� ����");

        weaponObjects[weaponData.weapon_index].SetActive(false);

    }


    public void UseWeapon()
    {
        
        if (currentWeapon != null)
            currentWeapon.Attack();
        else
            Debug.LogError("currentWeapon�� ���� �Ҵ���� ����");
    }

    public void ChangeWeapon()
    {
        weaponData.weapon_index = (weaponData.weapon_index + 1) % weaponObjects.Length;
        currentWeapon = weaponObjects[weaponData.weapon_index].GetComponent<IWeapon>();

    }
}
