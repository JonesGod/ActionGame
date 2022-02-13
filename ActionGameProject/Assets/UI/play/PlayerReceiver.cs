using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReceiver: MonoBehaviour
{
    float m_fHp = 100.0f;
    float m_fMp = 100.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //受傷扣血
    public void Hit(float fDamage)
    {
        
        UIMain.Instance().UpdateHpBar(m_fHp / 100.0f);
    }

    //施法扣魔
    public void Mana(float fMagic)
    {
        
        UIMain.Instance().UpdateMpBar(m_fMp / 100.0f);
    }
}
