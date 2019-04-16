using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerControler))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";

    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUiPrefab;

    [HideInInspector]
    public GameObject playerUiInstance;

    void Start()
    {
        // Disable components that should only be
        // active on the player that we control
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            // Disable player graphics for local player
            SetLayerRecusively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // Create PlayerUI
            playerUiInstance = Instantiate(playerUiPrefab);
            playerUiInstance.name = playerUiPrefab.name;

            // Configure PlayerUI.
            PlayerUI ui = playerUiInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.LogError("no player UI component on player prefab.");

            ui.SetController(GetComponent<PlayerControler>());

            GetComponent<Player>().PlayerSetup();
        }        
    }

    void SetLayerRecusively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecusively(child.gameObject, newLayer);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameManager.RegisterPlayer(netId, player);
    }

    // When we are destroyed
    void OnDisable()
    {
        Destroy(playerUiInstance);

        GameManager.instance.SetSceneCameraActive(true);

        // Deregister player
        GameManager.UnRegisterPlayer(transform.name);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

}
