using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public FSMBase myFSM;

    private float maxHealth;
    private float currentHealth;

    public event Action<float> OnHealthPctChanged = delegate { };

    // private void OnEnable()
    // {
    //     maxHealth = myFSM.data.hp;
    //     currentHealth = maxHealth;
    // }
    void Start()
    {
        maxHealth = myFSM.data.hp;
        currentHealth = maxHealth;
        //ModifyHealth(0);
    }

    //血量增減顯示
    public void ModifyHealth(float amout)
    {
        currentHealth -= amout;

        float currentHealthPct = (float)currentHealth / (float)maxHealth;
        OnHealthPctChanged(currentHealthPct);
    }
}
