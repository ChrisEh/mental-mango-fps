using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapon 
{
    public string name = "Glock";

    public int damage = 10;
    public float range = 100f;

    public float fireRate = 0f;
    public int maxBullets = 20;

    [HideInInspector]
    public int bullets = 20;

    public float reloadTime = 1;

    public GameObject Graphics;

    public void Shoot()
    {
        if (bullets != 0)
        {
            bullets = bullets--;
        }        
    }

    public PlayerWeapon()
    {
        bullets = maxBullets;
    }
}
