using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform thrusterFuelFill;

    private PlayerControler controller;
    public void SetController(PlayerControler controller)
    {
        this.controller = controller;
    }

    void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());
    }

    void SetFuelAmount(float amount)
    {
        thrusterFuelFill.localScale = new Vector3(1, amount, 1);
    }
}
