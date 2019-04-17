using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
        protected set { isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth = 0;

    public float GetHealthPct()
    {
        return (float)currentHealth / (float)maxHealth;
    }

    [SerializeField]
    private Behaviour[] disableOnDeath;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            // Switch camera.
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUiInstance.SetActive(true);
            GetComponent<WeapoinManager>().Reload();
        }

        CmdBroadcastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];

            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
        }

        firstSetup = false;

        SetDefaults();
    }

    [ClientRpc]
    public void RpcTakeDamage(int amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;        
                
        // Disable components.
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // Disable gameobjects.
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }
        
        // Disable collider.
        Collider collider = GetComponent<Collider>();

        if (collider != null)
            collider.enabled = false;

        // Spawn a death effect.
        GameObject gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gfxIns, 3f);

        // Switch camera.
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUiInstance.SetActive(false);
        }

        Debug.Log($"{transform.name} is dead.");

        // Call respawn.
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.01f);

        SetupPlayer();
        Debug.Log($"Player '{transform.name}' respawned.");
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;

        // Set components active.
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        // Set gameobjects active.
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        // Enable the collider.
        Collider collider = GetComponent<Collider>();

        if (collider != null)
            collider.enabled = true;

        //// Create spawn effect.
        //GameObject gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        //Destroy(gfxIns, 3f);
    }
}