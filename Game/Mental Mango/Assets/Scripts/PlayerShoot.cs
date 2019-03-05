using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private PlayerWeapon weapon;

    [SerializeField]
    private GameObject weaponGfx;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("No cam reference");
            enabled = false;
        }

        weaponGfx.layer = LayerMask.NameToLayer(weaponLayerName);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    [Client]
    void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask))
        {
            if (hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(hit.collider.name, weapon.damage);               
            }
        }
    }

    [Command]
    void CmdPlayerShot(string playerId, int damage)
    {
        Debug.Log(playerId + " has been shot.");

        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage);
    }
}
