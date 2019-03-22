using UnityEngine;
using UnityEngine.Networking;

public class WeapoinManager : NetworkBehaviour
{
    [SerializeField]
    private string weaponLayerName = "Weapon";
    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;
    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    void EquipWeapon(PlayerWeapon weapon)
    {
        currentWeapon = weapon;

        GameObject weaponIns = Instantiate(weapon.Graphics, weaponHolder.position, weaponHolder.rotation);
        weaponIns.transform.SetParent(weaponHolder);

        if (isLocalPlayer)
            weaponIns.layer = LayerMask.NameToLayer(weaponLayerName);
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
