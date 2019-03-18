using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeapoinManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    private PlayerWeapon currentWeapon;
    private WeapoinManager weaponManager;

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

        weaponManager = GetComponent<WeapoinManager>();
    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButton("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    //// Called on the server when a player shoots.
    //[Command]
    //void CmdOnShoot()
    //{
    //    DoShootEffect();
    //}

    //// Called on all clients when we need to do a shoot effect.
    //[ClientRpc]
    //void DoShootEffect()
    //{

    //}

    [Client]
    void Shoot()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //// We are shooting, call the methodf on the server.
        //CmdOnShoot();

        Debug.Log("Shoot");
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            if (hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage);               
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
