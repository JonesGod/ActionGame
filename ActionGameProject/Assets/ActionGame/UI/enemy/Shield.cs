using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shield : MonoBehaviour
{
    public RockMonsterFSM myFSM;

    private float maxShield;
    private float currentShield;

    public event Action<float> OnHealthPctChanged = delegate { };

    // private void OnEnable()
    // {
    //     maxHealth = myFSM.data.hp;
    //     currentHealth = maxHealth;
    // }
    void Start()
    {
        maxShield = myFSM.data.shield;
        currentShield = maxShield;
        //ModifyHealth(0);
    }

    //血量增減顯示
    public void ModifyShield(float amout)
    {
        currentShield -= amout;

        float currentHealthPct = (float)currentShield / (float)maxShield;
        OnHealthPctChanged(currentHealthPct);
    }
}
