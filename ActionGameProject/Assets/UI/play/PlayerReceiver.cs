using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReceiver: MonoBehaviour
{
    float m_fHp = 100.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hit(float fDamage)
    {
        Debug.Log("Hit " + fDamage);
        m_fHp -= fDamage;
        if(m_fHp < 0)
        {
            m_fHp = 0.0f;
        }
        UIMain.Instance().UpdateHpBar(m_fHp / 100.0f);
    }
}
