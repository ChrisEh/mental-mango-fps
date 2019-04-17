using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform thrusterFuelFill;

    [SerializeField]
    RectTransform healthbarFill;

    [SerializeField]
    Text ammoText;

    private Player player;
    private PlayerControler controller;
    private WeapoinManager weaponManager;
    
    public void SetPlayer(Player player)
    {
        this.player = player;
        controller = player.GetComponent<PlayerControler>();
        weaponManager = player.GetComponent<WeapoinManager>();
    }

    void Update()
    {
        if (thrusterFuelFill != null && controller != null)
        {
            SetFuelAmount(controller.GetThrusterFuelAmount());
        }

        if (player != null)
        {
            SetHealthAmount(player.GetHealthPct());
            SetAmmoAmount(weaponManager.GetCurrentWeapon().bullets);
        }
    }

    void SetFuelAmount(float amount)
    {
        thrusterFuelFill.localScale = new Vector3(1, amount, 1);
    }

    void SetHealthAmount(float amount)
    {
        healthbarFill.localScale = new Vector3(1, amount, 1);
    }

    void SetAmmoAmount(int amount)
    {
        ammoText.text = amount.ToString();
    }
}
