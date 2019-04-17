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
        
        if (currentWeapon.bullets < currentWeapon.maxBullets)
        {
            if (Input.GetButtonDown("Reload"))
            {
                weaponManager.Reload();
                return;
            }
        }

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

    // Called on the server when a player shoots.
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    // Called on all clients when we need to do a shoot effect.
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    // Called on the server when we hit something.
    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal)
    {
        RpcDoHitEffect(pos, normal);
    }

    // Called on all clients, here we can spawn the effects.
    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, pos, 
            Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }

    [Client]
    void Shoot()
    {
        if (!isLocalPlayer || weaponManager.isReloading)
            return;

        if (currentWeapon.bullets <= 0)
        {
            Debug.Log("Out of bullets.");
            return;
        }

        currentWeapon.bullets--;
        
        // We are shooting, call the methodf on the server.
        CmdOnShoot();

        Debug.Log("Shoot");
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            if (hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage);               
            }

            // We hit something, call the onHit method on the server.
            CmdOnHit(hit.point, hit.normal);
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
