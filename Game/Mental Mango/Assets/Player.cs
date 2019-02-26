using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public Health Model;
    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    void Awake()
    {
        SetDefaults();
    }

    public void TakeDamage(int amount)
    {
        Model.RemoveAmount(10);

        Debug.Log(transform.name + " now has " + currentHealth + " health.");
    }
    public void SetDefaults()
    {
        currentHealth = maxHealth;
    }

    public void UpdateValue()
    {
        currentHealth = Convert.ToInt32(Model.CurrentOwned);
        Debug.Log(transform.name + " now has " + currentHealth + " health.");
    }

    public void OnEnable()
    {
        UpdateValue();
        Model.OnValueChanged.AddListener(UpdateValue);
    }

    private void OnDisable()
    {
        Model.OnValueChanged.RemoveListener(UpdateValue);
    }
}