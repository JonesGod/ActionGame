using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{

    [SerializeField]
    private int maxHealth = 100;

    private int currentHealth;

    public event Action<float> OnHealthPctChanged = delegate { };

    private void OnEnable()
    {

        currentHealth = maxHealth;
    }

    //血量增減顯示
    public void ModifyHealth(int amout)
    {
        currentHealth += amout;

        float currentHealthPct = (float)currentHealth / (float)maxHealth;
        OnHealthPctChanged(currentHealthPct);
    }

    // 按按鍵-10hp
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ModifyHealth(-10);
    }
}
