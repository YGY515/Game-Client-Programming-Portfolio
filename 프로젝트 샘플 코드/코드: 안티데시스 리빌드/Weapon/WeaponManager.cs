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
            Debug.LogError($"{weaponObjects[weaponData.weapon_index].name} 오브젝트에 IWeapon 컴포넌트가 없음");

        weaponObjects[weaponData.weapon_index].SetActive(false);

    }


    public void UseWeapon()
    {
        
        if (currentWeapon != null)
            currentWeapon.Attack();
        else
            Debug.LogError("currentWeapon에 아직 할당되지 않음");
    }

    public void ChangeWeapon()
    {
        weaponData.weapon_index = (weaponData.weapon_index + 1) % weaponObjects.Length;
        currentWeapon = weaponObjects[weaponData.weapon_index].GetComponent<IWeapon>();

    }
}
